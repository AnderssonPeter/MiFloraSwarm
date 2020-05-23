const gulp = require('gulp'),
      zopfli = require('gulp-zopfli-green'),
      brotli = require('gulp-brotli'),
      zlib = require('zlib')

const fileTypesToCompress = ['html', 'css', 'js', 'json', 'svg'];
const paths = fileTypesToCompress.map(x => 'dist/**/*.' + x);

const compressGzip = () => 
    gulp.src(paths)
        .pipe(zopfli())
        .pipe(gulp.dest('dist/'));


const compressBrotli = () => 
    gulp.src(paths)
        .pipe(brotli({
            params: {
                [zlib.constants.BROTLI_PARAM_QUALITY]: zlib.constants.BROTLI_MAX_QUALITY,
            }
        }))
        .pipe(gulp.dest('dist/'));

exports.compress = gulp.parallel(compressGzip, compressBrotli);
