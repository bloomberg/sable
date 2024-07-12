# Why **Sable**?

The **Marten** team has done a phenomenal job with providing the foundational infrastructure required for managing database migrations. The command line tooling for that is made available via the `Marten.CommandLine` package, and works just fine. 
With a connection string that is sufficiently privileged to execute migration scripts, the `marten-patch` and `marten-apply` commands can easily be used to carry out the process. However, in a corporate environment like Bloomberg, this approach is not feasible. But why not?
Well, for local development, it's not an issue, but for other environments like dev, alpha, beta, and prod, we've encountered some limitations  because of the following reasons:
- There's a standard process for executing database migrations scripts. An engineer can't just point to a database to run migrations, but needs to submit a ticket that must be approved by a manager/team lead before the script can be executed.
- An application will often be deployed to multiple environments in a sequential deployment pipeline (e.g., dev -> alpha -> beta -> prod). Furthermore, these deployments won't happen in a compressed time frame. You want to test things in one environment before proceeding to the next, so it might take at least a week before moving from one environment the next. As a result, a lot of questions/concerns will surface:
    - How to know which scripts have already been applied to which environments?
    - How to make sure scripts are applied in the same order for each environment in the deployment pipeline?
    - How to guard against human errors like applying a script in an environment more than once? Errors like this can lead to costly outcomes like an accidentally dropped table.

**Sable** solves all of these problem by taking a simple, intuitive, and minimally-invasive approach. Curious to know how it works? See [How Sable Works](/reference/how-sable-works) to learn more.