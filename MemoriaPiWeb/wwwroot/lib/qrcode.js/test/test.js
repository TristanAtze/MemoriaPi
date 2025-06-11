var qr = require('./index')
  , fs = require('fs')
  , png = require('pngjs').PNG

fs.createReadStream('test-detect.png')
  .pipe(new png({filterType: 4}))
  .on('parsed', function() {

    var im = this
    im.getImageData = function(){ 
      return {data: im.data}
    }

    qr.detect(im, function(err, data){

      console.log("!!",err, data);
    })
})
