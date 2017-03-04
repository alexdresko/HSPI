const gulp = require('gulp');
const browserSync = require('browser-sync').create();
const shell = require('gulp-shell')

gulp.task('default', () => {});

gulp.task('browser-sync', () => {
    browserSync.init({
        server: {
            baseDir: "./docs/build/html",
        },
        files: "./docs/build/html/*.html"
    });
});

gulp.task('watch', ['browser-sync'], () => {
    gulp.watch('./docs/source/**/*.rst', ['build']);
});

gulp.task('build', shell.task('docs\\make.bat html'));