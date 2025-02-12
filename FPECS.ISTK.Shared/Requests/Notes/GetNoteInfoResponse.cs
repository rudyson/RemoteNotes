using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPECS.ISTK.Shared.Requests.Notes;
public class GetNoteInfoResponse
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}