var gulp = require('gulp'),
    bowerFiles = require('main-bower-files'),
    rm = require('gulp-rm');

gulp.task('bower', function() {
  return gulp.src(bowerFiles())
    .pipe(gulp.dest('wwwroot/lib'));
});

gulp.task('clean', function() {
  return gulp.src('wwwroot/**/*', {read: false})
    .pipe(rm());
});

gulp.task('scripts', function() {
  return gulp.src('Scripts/*.js')
      .pipe(gulp.dest('wwwroot/js'));
});

gulp.task('styles', function() {
  return gulp.src('Styles/*.css')
      .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('default', ['bower', 'scripts', 'styles']);