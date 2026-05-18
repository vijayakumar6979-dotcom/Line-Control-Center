using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>JCAS customer lookup from the jcas_customer_tbl table (PostgreSQL).</summary>
public sealed partial class JcasCustomerTbl : Entity<JcasCustomerId>
{
    private JcasCustomerTbl() { }

    /// <summary>Creates a <see cref="JcasCustomerTbl"/> instance from persistence.</summary>
    public static JcasCustomerTbl From(int customerNoId, string? customerName, DateTime updatedDatetime)
    {
        return new JcasCustomerTbl
        {
            Id              = JcasCustomerId.From(customerNoId),
            CustomerNoId    = customerNoId,
            CustomerName    = customerName,
            UpdatedDatetime = updatedDatetime
        };
    }

    /// <summary>Customer surrogate key.</summary>
    public int CustomerNoId { get; private set; }

    /// <summary>Customer display name.</summary>
    public string? CustomerName { get; private set; }

    /// <summary>Last updated timestamp.</summary>
    public DateTime UpdatedDatetime { get; private set; }

    /// <summary>Navigation: JCAS records for this customer.</summary>
    public ICollection<JcasMainTbl> JcasMainTbls { get; private set; } = new List<JcasMainTbl>();
}