const gulp = require('gulp'),
      zopfli = require('gulp-zopfli-green'),
      brotli = require('gulp-brotli'),   
      zlib = require('zlib')

const compressGzip = () => 
    gulp.src(['dist/miflora-gateway/*.html', 'dist/miflora-gateway/*.css', 'dist/miflora-gateway/*.js', 'dist/miflora-gateway/*.json'])
        .pipe(zopfli())
        .pipe(gulp.dest('dist/miflora-gateway/'));

        
const compressBrotli = () => 
    gulp.src(['dist/miflora-gateway/*.html', 'dist/miflora-gateway/*.css', 'dist/miflora-gateway/*.js', 'dist/miflora-gateway/*.json'])
        .pipe(brotli({
            params: {                
                [zlib.constants.BROTLI_PARAM_QUALITY]: zlib.constants.BROTLI_MAX_QUALITY,
            }
        }))
        .pipe(gulp.dest('dist/miflora-gateway/'));

exports.compress = gulp.parallel(compressGzip, compressBrotli);