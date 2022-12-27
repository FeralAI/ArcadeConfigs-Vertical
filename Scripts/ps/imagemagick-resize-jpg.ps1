Get-ChildItem "." -File | Foreach { magick.exe convert "$($_.fullname)" -filter Point -resize 960x960! "$($_.fullname)" }
