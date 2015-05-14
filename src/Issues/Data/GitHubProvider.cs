using Octokit;

namespace Issues.Data
{
    public interface IGitHubProvider
    {
        GitHubClient GetClient();
    }

    public class GitHubProvider : IGitHubProvider
    {
        private readonly string _token;

        public GitHubProvider(string token)
        {
            _token = token;
        }

	    public GitHubClient GetClient()
        {
            var client = new GitHubClient(new ProductHeaderValue("DNX-Issues"));
            client.Credentials = new Credentials(_token);
            return client;
        }
    }
}