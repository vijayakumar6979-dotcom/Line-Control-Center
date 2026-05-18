# Shift Gauge Fix — Root Cause Analysis & Resolution

## Problem Statement
Shift gauges (Yield RTY and Plan vs Actual) were not displaying any data despite records existing in the database for the selected customer/division/family/date.

---

## Root Cause Identified

### Issue 1: TestLoopCount Filter Too Restrictive ❌
**Location:** `LineControlCenter.UI\Models\CardModel.cs` (lines 12, 18)

**Original Code:**
```csharp
public int PassCount => Records.Count(x => x.TestStatus == "P" && x.TestLoopCount == 1);
public int TestedCount => Records.Count(x => x.TestLoopCount == 1);
```

**Problem:**
- Filter required `TestLoopCount == 1` (exact match)
- If database has `TestLoopCount = NULL`, `0`, or any other value, **all records were excluded**
- Result: `PassCount = 0`, `TestedCount = 0`, `YieldRate = 0` → **gauges invisible**

**Fix Applied:**
```csharp
public int PassCount => Records.Count(x => 
    x.TestStatus == "P" && (x.TestLoopCount == null || x.TestLoopCount <= 1));

public int TestedCount => Records.Count(x => 
    x.TestLoopCount == null || x.TestLoopCount <= 1);
```

**Rationale:**
- Include first-pass tests: `NULL`, `0`, and `1`
- Exclude only retests: `TestLoopCount > 1`
- Handles real-world data where `TestLoopCount` may be `NULL` or `0` for initial tests

---

### Issue 2: SVG Arc Path Bugs (Previously Fixed)

**Location:** `LineControlCenter.UI\Components\Shared\HalfCircleGauge.razor`

**Problems:**
1. **Degenerate arc at 0%** — SVG path from `M 20,100` to `20,100` (start == end) is ignored by browsers
2. **Backwards arc > 50%** — Missing `large-arc-flag` caused arc to draw short path instead of long path
3. **Needle stuck at 0** — Overlapped track start when value was exactly 0

**Fixes:**
```csharp
// Clamp arc to [0.5%, 99.5%] so start != end
var pct = Math.Clamp(_percent, 0.5, 99.5);
var largeArc = pct > 50 ? 1 : 0;  // Use large arc for > 50%
return $"M 20,100 A 80,80 0 {largeArc} 1 {Fmt(endX)},{Fmt(endY)}";
```

---

## Diagnostic Tools Added

### 1. Server-Side Logging
**Location:** `LineControlCenter.Application\Queries\TestData\GetBkTestDataQuery.cs`

Added structured logging to track:
- Query parameters (Customer, Division, Family, Shift, Date)
- Total record count returned
- TestLoopCount distribution (NULL / 0 / 1 / >1)

**Sample Output:**
```
GetBkTestData: Customer=ABC, Division=XYZ, Family=Widget, Shift=Morning, Date=01/16/2025, 
RecordCount=150, TestLoopCount_Null=50, TestLoopCount_0=10, TestLoopCount_1=85, TestLoopCount_Other=5
```

### 2. Client-Side Diagnostic Panel
**Location:** `LineControlCenter.UI\Components\Pages\Home.razor`

Added inline diagnostic panel below each shift card showing:
- Total records loaded
- TestLoopCount breakdown: `NULL=X, 0=X, 1=X, >1=X`
- Warning when no data loaded

**Visual Example:**
```
📊 150 records loaded · LoopCount: NULL=50, 0=10, 1=85, >1=5
```

---

## Testing Instructions

### 1. Run the Diagnostic SQL Query
Execute `diagnose_testloopcount.sql` against your MSSQL database to verify data:

```sql
-- Check TestLoopCount distribution
SELECT 
    TestLoopCount,
    COUNT(*) AS RecordCount,
    COUNT(CASE WHEN TestStatus = 'P' THEN 1 END) AS PassCount
FROM db_owner.BK_Test_Tar_RawData
WHERE 
    ShiftDate = '01/16/2025'  -- YOUR SELECTED DATE
    AND Shift IN ('Morning', 'Night')
GROUP BY TestLoopCount
ORDER BY TestLoopCount;
```

**Expected:** You should see records with `TestLoopCount = NULL`, `0`, or `1`

### 2. Launch Dashboard
1. Select Customer / Division / Family
2. Set Shift Date (MM/dd/yyyy format)
3. Set Planned Qty (e.g., 100)
4. Click **LAUNCH DASHBOARD**

### 3. Verify Gauges
Check both shift cards (Morning & Night):
- ✅ **Yield (RTY) gauge** shows percentage arc and needle
- ✅ **Plan vs Actual gauge** shows actual count vs planned
- ✅ **Diagnostic panel** shows `X records loaded` with TestLoopCount breakdown
- ❌ If "NO DATA LOADED" appears → check date format, shift values, or database connection

### 4. Check Logs
Open Visual Studio Output window → Show output from: **Debug**

Look for:
```
GetBkTestData: Customer=..., RecordCount=..., TestLoopCount_Null=...
```

If `RecordCount=0`:
- Verify date is in `MM/dd/yyyy` format in database `ShiftDate` column
- Verify `Shift` column contains `"Morning"` or `"Night"` (case-sensitive)
- Check `TestStatus != "A"` (aborted tests are excluded)

---

## Files Modified

| File | Change |
|------|--------|
| `LineControlCenter.UI\Models\CardModel.cs` | Fixed TestLoopCount filter to handle NULL/0/1 |
| `LineControlCenter.UI\Components\Shared\HalfCircleGauge.razor` | Fixed SVG arc path bugs (previously done) |
| `LineControlCenter.Application\Queries\TestData\GetBkTestDataQuery.cs` | Added structured logging |
| `LineControlCenter.UI\Components\Pages\Home.razor` | Added diagnostic panel |

---

## Next Steps

1. **Remove diagnostic panels** after confirming gauges work (optional — can leave for troubleshooting)
2. **Verify TestLoopCount semantics** with business stakeholders:
   - Should `NULL` be treated as first-pass? ✅ (current assumption)
   - Should `0` be treated as first-pass? ✅ (current assumption)
3. **Monitor logs** for TestLoopCount distribution patterns across different products

---

## Database Schema Note

**Table:** `db_owner.BK_Test_Tar_RawData` (MSSQL: `JBK_TE` on `mypenm0plfsvr`)

**Key Columns:**
- `ShiftDate` (varchar(10)) — format: `MM/dd/yyyy` (e.g., `"01/16/2025"`)
- `Shift` (varchar(7)) — values: `"Morning"`, `"Night"`
- `TestLoopCount` (tinyint, nullable) — `NULL`, `0`, `1`, `2+`
- `TestStatus` (char(1)) — `"P"` (Pass), `"F"` (Fail), `"A"` (Aborted)

---

**Build Status:** ✅ All changes compiled successfully
**Ready for testing:** Yes — deploy and verify with real data
