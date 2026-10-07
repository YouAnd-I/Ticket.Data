namespace Ticket.Data;

public enum TicketPriority
{
    Auto,
    Urgent,
    NoRush,
    Report,
}

public partial struct TicketRecord
{
    public string Id;
    public string Requester;
}

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

public partial struct PriorityClassifyRequested
{
    public string TicketId;
    public string Text;

    // The editable categories to pick from, plus today's date — the model
    // has no clock of its own, and availability is time-based.
    public TicketCategory[] Categories;
    public string NowUtc;
}

public partial struct PriorityClassified
{
    public string TicketId;
    public TicketPriority? Priority;
    public string? Category;
    public bool Offline;
}

public partial struct PriorityClassifiedAck { }
