using LineControlCenter.Domain.Primitives;

namespace LineControlCenter.Domain.Compliance;

/// <summary>JCAS category lookup from the jcas_category_tbl table (PostgreSQL).</summary>
public sealed partial class JcasCategoryTbl : Entity<JcasCategoryId>
{
    private JcasCategoryTbl() { }

    /// <summary>Creates a <see cref="JcasCategoryTbl"/> instance from persistence.</summary>
    public static JcasCategoryTbl From(int categoryNoId, string? categoryName, DateTime updatedDatetime)
    {
        return new JcasCategoryTbl
        {
            Id              = JcasCategoryId.From(categoryNoId),
            CategoryNoId    = categoryNoId,
            CategoryName    = categoryName,
            UpdatedDatetime = updatedDatetime
        };
    }

    /// <summary>Category surrogate key.</summary>
    public int CategoryNoId { get; private set; }

    /// <summary>Category display name.</summary>
    public string? CategoryName { get; private set; }

    /// <summary>Last updated timestamp.</summary>
    public DateTime UpdatedDatetime { get; private set; }

    /// <summary>Navigation: JCAS records in this category.</summary>
    public ICollection<JcasMainTbl> JcasMainTbls { get; private set; } = new List<JcasMainTbl>();
}