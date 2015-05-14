$(function() {
  var milestone = window.location.pathname.split('/').pop();

  var totalIssues = 0;

  function gotIssues(repo, issues) {
    if (!(issues && issues.length)) return;
    totalIssues += issues.length;
    $('#main-header').text('Total: ' + totalIssues + " open issues");
    $('#issues').append('<div class="row"><h3>' + repo.name + ' (' + issues.length + ')</h3><ul id="ul-' + repo.name + '"></ul></div>');
    var $ul = $('#ul-' + repo.name);
    issues.forEach(function(issue) {
      $ul.append('<li><a href="' + issue.htmlUrl + '" target="_blank">' + issue.title + '</a></li>');
    });
  }

  function gotRepos(repos) {
    repos.forEach(function(repo) {
      $.get('/query/issues/' + repo.name + '/' + milestone)
        .then(function(issues) {
          gotIssues(repo, issues);
        });
    });
  };

  $.get('/query/repos')
    .then(gotRepos);
});