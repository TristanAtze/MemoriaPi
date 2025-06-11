var qrcode = require('./src/qrcode')


module.exports = {
  encode : function(data, cb){

  }

, decode : function(im, cb){

  }

, detect : function(im, cb){
  qrcode.process(im, cb);
  }
}
