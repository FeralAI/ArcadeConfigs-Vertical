Get-ChildItem "C:\retrobat\roms\videos" -File | Foreach { Write-Output $($_.name) }
Get-ChildItem "C:\retrobat\roms\videos" -File | Foreach { .\ffmpeg.exe -i "C:\retrobat\roms\videos\cropped\$($_.name)" -filter:v "crop=360:480:247:0" "C:\retrobat\roms\videos\cropped-$($_.name)" }
