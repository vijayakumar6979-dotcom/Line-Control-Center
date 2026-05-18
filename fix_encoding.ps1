$f = "C:\Documents\Project\LineControlCenter\LineControlCenter.UI\Components\Dialogs\UserGuideDialog.razor"
$c = [System.IO.File]::ReadAllText($f, [System.Text.Encoding]::UTF8)

# em-dash variants
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x80,0x93)), '-')   # en-dash
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x80,0x94)), '-')   # em-dash
# right arrow
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x86,0x92)), '->')
# minus/subtraction
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x88,0x92)), '-')
# division sign
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xC3,0xB7)), '/')
# multiplication sign
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xC3,0x97)), 'x')
# greater-than-or-equal
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x89,0xA5)), '>=')
# middle dot
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xC2,0xB7)), '.')
# check mark
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x9C,0x94)), '(ok)')
# bullet / black circle
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x97,0x8F)), '*')
# clipboard emoji
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xF0,0x9F,0x93,0x8B)), '[NOTE]')
# non-breaking space
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xC2,0xA0)), ' ')
# left double quotation
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x80,0x9C)), '"')
# right double quotation
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x80,0x9D)), '"')
# right single quotation / apostrophe
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x80,0x99)), "'")
# ellipsis
$c = $c.Replace([System.Text.Encoding]::UTF8.GetString([byte[]](0xE2,0x80,0xA6)), '...')

[System.IO.File]::WriteAllText($f, $c, [System.Text.Encoding]::UTF8)
Write-Host "Done"
