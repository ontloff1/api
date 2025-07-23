using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiasApi.Models;

[Table("fias_addr_obj")]
public class AddrObj
{
    [Key]
    public Guid AOGUID { get; set; }

    public string? FORMALNAME { get; set; }
    public string? REGIONCODE { get; set; }
    public Guid AOID { get; set; }
    public int AOLEVEL { get; set; }
    public string? SHORTNAME { get; set; }
    public string? OFFNAME { get; set; }
}
