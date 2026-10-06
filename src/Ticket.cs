namespace Ticket.Data;

public enum TicketPriority
{
    Auto,
    Urgent,
    NoRush,
    Report,
}

// A ticket is state, not a message: it outlives the reply. The world keeps the
// ticket; this component marks the ticket's own entity.
// (Named TicketRecord, not Ticket: a type named Ticket would hide the Ticket
// namespace everywhere inside Ticket.* modules.)
public partial struct TicketRecord
{
    public string Id;
    public string Requester;
}

// Everything a screen needs to draw the ticket card. Plain data only: names and
// URLs as display text, never SDK objects or platform ids.
public partial struct TicketView
{
    public string TicketId;
    public bool Exists;
    public string? Title;
    public string? Description;
    public string Priority;
    public bool AutoClassified;
    public bool ClassifierOffline;
    public string? AttachmentUrl;
    public string? Assignee;
    public int NoteCount;
    public string? SolutionTitle;
    public string? SolutionText;
    public string? SolutionImage;
    public DateTimeOffset CreatedAtUtc;
    public string Status;
}

// /it — create a ticket. Requester and Assignee are display text the adapter
// chose (Discord sends mentions); the world never interprets them.
public partial struct TicketCreate
{
    public string? Title;
    public string? Description;
    public TicketPriority RequestedPriority;
    public string? AttachmentUrl;
    public string Requester;
    public string? Assignee;
}

public partial struct TicketCreated
{
    public TicketView View;
}

// Status buttons — customId itstatus:<status>:<ticketId>
public partial struct TicketStatusChange
{
    public string TicketId;
    public string Status;
    public string ByUser;
}

public partial struct TicketChanged
{
    public TicketView View;
}

// Add note — itnote:<ticketId> → notemodal:<ticketId>
public partial struct TicketNote
{
    public string TicketId;
    public string Note;
    public string ByUser;
}

public partial struct TicketNoteAdded
{
    public TicketView View;
    public string Note;
    public int NoteCount;
}

// Confidential report — itreport:<ticketId> → reportmodal:<ticketId>
public partial struct TicketReport
{
    public string TicketId;
    public string Complaint;
    public string Action;
    public string? FileUrl;
    public bool Anonymous;
    public string ByUser;
}

public partial struct TicketReported
{
    public TicketView View;
}

// World → classifier adapter: the system asks for a priority classification.
// Emitted as a component; delivered through IWorldClient.Subscribe.
public partial struct PriorityClassifyRequested
{
    public string TicketId;
    public string Text;
}

// classifier adapter → world: the classification result. Offline = the classifier
// could not be reached (the system then defaults to urgent).
public partial struct PriorityClassified
{
    public string TicketId;
    public TicketPriority? Priority;
    public bool Offline;
}

public partial struct PriorityClassifiedAck { }
