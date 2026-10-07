using System.Text.Json.Nodes;

namespace Ticket.Data;

// A saved how-to for a solved ticket, offered back on similar new tickets.
public sealed record TicketSolution(string Title, string? Text, string? Image);

// One editable classification: the classifier picks from these, and skills
// reference them. Description is the classifier guidance, written in the sheet.
public sealed record TicketCategory(string Slug, string Description);

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

    // The editable categories (sheet-managed in Postgres; empty for files).
    IReadOnlyList<TicketCategory> Categories();

    // Who is reachable for a ticket of this category at this moment:
    // specialists for the category first, then anyone active and not absent.
    TicketRoute Route(string? categorySlug, DateTimeOffset nowUtc);

    void AppendStatus(string user, string id, string status);

    // Appends the note and returns how many notes the ticket now has.
    int AppendNote(string user, string id, string note);

    void AppendReport(string user, string id, string complaint, string action,
        bool anonymous, string? file);
}
