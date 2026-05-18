using LineControlCenter.Domain;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Read-only view of the jbk_te PostgreSQL database (public.bk_uph_tar).</summary>
public interface IJbkTeDbContext
{
    /// <summary>BK UPH TAR test records — equivalent of MSSQL BK_Test_Tar_RawData.</summary>
    IQueryable<BkUphTar> BkUphTars { get; }
}
