using System.Text.Json.Nodes;

namespace Ticket.Data;

// A saved how-to for a solved ticket, offered back on similar new tickets.
public sealed record TicketSolution(string Title, string? Text, string? Image);

// One urgency level. Description is the classifier guidance for that level,
// written in the sheet — the model reads it to pick.
public sealed record PriorityOption(string Code, string Description);

// The out-of-the-box urgency guidance: seeded into Postgres once, and used
// directly whenever the store offers no priorities of its own.
public static class PriorityGuidance
{
    public static readonly IReadOnlyList<PriorityOption> Defaults =
    [
        new("urgent", "Something is broken, failing, or blocking the user right now"),
        new("no-rush", "A question or a request that can wait; nothing is failing"),
    ];
}

// One IT staff member who is on duty right now. Handles is free text the
// classifier reads ("wifi, VPN, anything printing"), written in the sheet.
public sealed record StaffMember(string StaffId, string Name, string Handles);

// Who should be told about a ticket right now: plain Discord user ids, empty
// when nobody is reachable.
public sealed record TicketRoute(IReadOnlyList<ulong> StaffIds)
{
    public static readonly TicketRoute None = new([]);
}

// Where the ticket feature keeps its state. The system writes through this
// from inside the tick, and the composition root picks the implementation:
// files on disk (TicketStore in Ticket.System.Frent, the default) or Postgres
// (NpgsqlTicketStore in Ticket.Adapter.Npgsql, for Neon). Plain data in and
// out only, which is why it can live in Data.
public interface ITicketStore
{
    // Create: one audit-log line + the ticket itself (open, notes empty).
    void Append(string user, string id, string priority, bool auto, bool offline,
        string? title, string? desc, string? file, string? assigned);

    // The ticket as the same JSON the file store writes (id, user, created,
    // priority, auto, offline, assigned, title, description, file, status,
    // notes) — TicketSystem.View hydrates from it.
    JsonNode? LoadJson(string id);

    // Best how-to whose title/text mentions words from the query, if any.
    TicketSolution? BestSolution(string? query);

    // The editable urgency levels (sheet-managed in Postgres; empty for files).
    IReadOnlyList<PriorityOption> Priorities();

    // Staff who are active and not absent at this moment, with their
    // sheet-written handles text — offered to the classifier as choices.
    IReadOnlyList<StaffMember> AvailableStaff(DateTimeOffset nowUtc);

    // Fallback for when the classifier is offline or picked nobody: any
    // staff member on duty, most senior id first.
    TicketRoute Route(DateTimeOffset nowUtc);

    void AppendStatus(string user, string id, string status);

    // Appends the note and returns how many notes the ticket now has.
    int AppendNote(string user, string id, string note);

    void AppendReport(string user, string id, string complaint, string action,
        bool anonymous, string? file);
}
