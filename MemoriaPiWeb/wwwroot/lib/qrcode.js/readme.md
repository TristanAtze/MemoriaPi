# Javascript QR Codes

Read and Generate QR codes in js - either in node, or in the browser with browserify.

A fork of [jsqrcode](https://github.com/LazarSoft/jsqrcode) which is a port of [ZXing](http://code.google.com/p/zxing)


## Usage




Set qrcode.callback to function "func(data)", where data will get the decoded information.

Decode image with: qrcode.decode(url or DataURL).
Decode from canvas with "qr-canvas" ID: qrcode.decode()

For webcam qrcode decoding (included in the test.html) you will need the camcanvas.swf from http://www.taboca.com/p/camcanvas/
