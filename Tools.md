# Tools

## imagemagick
https://imagemagick.org/script/download.php

Re-Canvas size images with centering:

> For handling a directory of JPGs, and using white as the background:

__Command:__ ```magick mogrify -extent 640x640 -gravity Center -fill white *.jpg```

> For PNGs, and having a transparent background

__Command:__ ```magick mogrify -extent 216x185 -gravity Center -background none *.png```


C:\Users\ntdls\Desktop\TestLava