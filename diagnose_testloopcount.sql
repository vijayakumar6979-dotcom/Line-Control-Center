-- Diagnostic query: Check TestLoopCount distribution
-- Run this against JBK_TE database on mypenm0plfsvr

SELECT 
    TestLoopCount,
    COUNT(*) AS RecordCount,
    COUNT(CASE WHEN TestStatus = 'P' THEN 1 END) AS PassCount,
    COUNT(CASE WHEN TestStatus = 'F' THEN 1 END) AS FailCount
FROM db_owner.BK_Test_Tar_RawData
WHERE 
    ShiftDate = '01/16/2025'  -- Replace with your selected date (MM/dd/yyyy format)
    AND Shift IN ('Morning', 'Night')
    AND Customer IS NOT NULL
    AND Division IS NOT NULL
    AND Family IS NOT NULL
GROUP BY TestLoopCount
ORDER BY TestLoopCount;

-- Also check a sample of recent records
SELECT TOP 10
    SerialNumber,
    Customer,
    Division,
    Family,
    Shift,
    ShiftDate,
    TestStatus,
    TestLoopCount
FROM db_owner.BK_Test_Tar_RawData
WHERE 
    ShiftDate >= '01/15/2025'
ORDER BY StartDateTime DESC;
