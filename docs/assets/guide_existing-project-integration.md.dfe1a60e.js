import{_ as s,o as a,c as n,Q as o}from"./chunks/framework.b8722102.js";const u=JSON.parse('{"title":"Existing Project Integration","description":"","frontmatter":{},"headers":[],"relativePath":"guide/existing-project-integration.md","filePath":"guide/existing-project-integration.md"}'),l={name:"guide/existing-project-integration.md"},e=o(`<h1 id="existing-project-integration" tabindex="-1">Existing Project Integration <a class="header-anchor" href="#existing-project-integration" aria-label="Permalink to &quot;Existing Project Integration&quot;">​</a></h1><p>So, you&#39;ve read the <a href="./../introduction/getting-started.html">Getting Started</a> guide, and may be wondering how to integrate <strong>Sable</strong> into an existing project. It&#39;s very simple.</p><h2 id="prerequisites" tabindex="-1">Prerequisites <a class="header-anchor" href="#prerequisites" aria-label="Permalink to &quot;Prerequisites&quot;">​</a></h2><ul><li>If you have yet to do so, make sure to read the <a href="./../introduction/getting-started.html">Getting Started</a> guide. The process for integrating Sable into an existing project is very similar to what is described there, but with a slight twist, so everything learned there will be applicable for existing projects.</li><li>Make sure all of your Postgres databases across every environment where your code is running have already converged to the same latest state.</li></ul><p>Once those prerequisites are met, you&#39;re all set to go.</p><h2 id="application-configuration" tabindex="-1">Application Configuration <a class="header-anchor" href="#application-configuration" aria-label="Permalink to &quot;Application Configuration&quot;">​</a></h2><ul><li>Ensure support for sable is configured. Your configuration should look something like the following:</li></ul><div class="language-c# vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">c#</span><pre class="shiki github-dark vp-code-dark"><code><span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">Marten</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">Oakton</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">BloombergLP</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Sable</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Extensions</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">BloombergLP</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Sable</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Samples</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Core</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">Weasel</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Core</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"></span>
<span class="line"><span style="color:#F97583;">var</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">builder</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> WebApplication.</span><span style="color:#B392F0;">CreateBuilder</span><span style="color:#E1E4E8;">(args);</span></span>
<span class="line"><span style="color:#E1E4E8;">builder.Host.</span><span style="color:#B392F0;">ApplyOaktonExtensions</span><span style="color:#E1E4E8;">();</span></span>
<span class="line"><span style="color:#E1E4E8;">builder.Services.</span><span style="color:#B392F0;">AddMartenWithSableSupport</span><span style="color:#E1E4E8;">(</span><span style="color:#B392F0;">_</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=&gt;</span></span>
<span class="line"><span style="color:#E1E4E8;">{</span></span>
<span class="line"><span style="color:#E1E4E8;">    </span><span style="color:#F97583;">var</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">options</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">new</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">StoreOptions</span><span style="color:#E1E4E8;">();</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.</span><span style="color:#B392F0;">Connection</span><span style="color:#E1E4E8;">(builder.Configuration[</span><span style="color:#9ECBFF;">&quot;Databases:Books:BasicTier&quot;</span><span style="color:#E1E4E8;">]);</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.DatabaseSchemaName </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> </span><span style="color:#9ECBFF;">&quot;books&quot;</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.AutoCreateSchemaObjects </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> AutoCreate.None;</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.Schema.</span><span style="color:#B392F0;">For</span><span style="color:#E1E4E8;">&lt;</span><span style="color:#B392F0;">Book</span><span style="color:#E1E4E8;">&gt;()</span></span>
<span class="line"><span style="color:#E1E4E8;">        .</span><span style="color:#B392F0;">Index</span><span style="color:#E1E4E8;">(</span><span style="color:#B392F0;">x</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=&gt;</span><span style="color:#E1E4E8;"> x.Contents);</span></span>
<span class="line"><span style="color:#E1E4E8;">    </span><span style="color:#F97583;">return</span><span style="color:#E1E4E8;"> options;</span></span>
<span class="line"><span style="color:#E1E4E8;">});</span></span>
<span class="line"></span>
<span class="line"><span style="color:#F97583;">var</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">app</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> builder.</span><span style="color:#B392F0;">Build</span><span style="color:#E1E4E8;">();</span></span>
<span class="line"><span style="color:#E1E4E8;">app.</span><span style="color:#B392F0;">MapGet</span><span style="color:#E1E4E8;">(</span><span style="color:#9ECBFF;">&quot;/&quot;</span><span style="color:#E1E4E8;">, () </span><span style="color:#F97583;">=&gt;</span><span style="color:#E1E4E8;"> </span><span style="color:#9ECBFF;">&quot;💪🏾&quot;</span><span style="color:#E1E4E8;">);</span></span>
<span class="line"></span>
<span class="line"><span style="color:#F97583;">return</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">await</span><span style="color:#E1E4E8;"> app.</span><span style="color:#B392F0;">RunOaktonCommands</span><span style="color:#E1E4E8;">(args);</span></span></code></pre><pre class="shiki github-light vp-code-light"><code><span class="line"><span style="color:#D73A49;">using</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">Marten</span><span style="color:#24292E;">;</span></span>
<span class="line"><span style="color:#D73A49;">using</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">Oakton</span><span style="color:#24292E;">;</span></span>
<span class="line"><span style="color:#D73A49;">using</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">BloombergLP</span><span style="color:#24292E;">.</span><span style="color:#6F42C1;">Sable</span><span style="color:#24292E;">.</span><span style="color:#6F42C1;">Extensions</span><span style="color:#24292E;">;</span></span>
<span class="line"><span style="color:#D73A49;">using</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">BloombergLP</span><span style="color:#24292E;">.</span><span style="color:#6F42C1;">Sable</span><span style="color:#24292E;">.</span><span style="color:#6F42C1;">Samples</span><span style="color:#24292E;">.</span><span style="color:#6F42C1;">Core</span><span style="color:#24292E;">;</span></span>
<span class="line"><span style="color:#D73A49;">using</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">Weasel</span><span style="color:#24292E;">.</span><span style="color:#6F42C1;">Core</span><span style="color:#24292E;">;</span></span>
<span class="line"></span>
<span class="line"><span style="color:#D73A49;">var</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">builder</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> WebApplication.</span><span style="color:#6F42C1;">CreateBuilder</span><span style="color:#24292E;">(args);</span></span>
<span class="line"><span style="color:#24292E;">builder.Host.</span><span style="color:#6F42C1;">ApplyOaktonExtensions</span><span style="color:#24292E;">();</span></span>
<span class="line"><span style="color:#24292E;">builder.Services.</span><span style="color:#6F42C1;">AddMartenWithSableSupport</span><span style="color:#24292E;">(</span><span style="color:#6F42C1;">_</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=&gt;</span></span>
<span class="line"><span style="color:#24292E;">{</span></span>
<span class="line"><span style="color:#24292E;">    </span><span style="color:#D73A49;">var</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">options</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">new</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">StoreOptions</span><span style="color:#24292E;">();</span></span>
<span class="line"><span style="color:#24292E;">    options.</span><span style="color:#6F42C1;">Connection</span><span style="color:#24292E;">(builder.Configuration[</span><span style="color:#032F62;">&quot;Databases:Books:BasicTier&quot;</span><span style="color:#24292E;">]);</span></span>
<span class="line"><span style="color:#24292E;">    options.DatabaseSchemaName </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> </span><span style="color:#032F62;">&quot;books&quot;</span><span style="color:#24292E;">;</span></span>
<span class="line"><span style="color:#24292E;">    options.AutoCreateSchemaObjects </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> AutoCreate.None;</span></span>
<span class="line"><span style="color:#24292E;">    options.Schema.</span><span style="color:#6F42C1;">For</span><span style="color:#24292E;">&lt;</span><span style="color:#6F42C1;">Book</span><span style="color:#24292E;">&gt;()</span></span>
<span class="line"><span style="color:#24292E;">        .</span><span style="color:#6F42C1;">Index</span><span style="color:#24292E;">(</span><span style="color:#6F42C1;">x</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=&gt;</span><span style="color:#24292E;"> x.Contents);</span></span>
<span class="line"><span style="color:#24292E;">    </span><span style="color:#D73A49;">return</span><span style="color:#24292E;"> options;</span></span>
<span class="line"><span style="color:#24292E;">});</span></span>
<span class="line"></span>
<span class="line"><span style="color:#D73A49;">var</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">app</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> builder.</span><span style="color:#6F42C1;">Build</span><span style="color:#24292E;">();</span></span>
<span class="line"><span style="color:#24292E;">app.</span><span style="color:#6F42C1;">MapGet</span><span style="color:#24292E;">(</span><span style="color:#032F62;">&quot;/&quot;</span><span style="color:#24292E;">, () </span><span style="color:#D73A49;">=&gt;</span><span style="color:#24292E;"> </span><span style="color:#032F62;">&quot;💪🏾&quot;</span><span style="color:#24292E;">);</span></span>
<span class="line"></span>
<span class="line"><span style="color:#D73A49;">return</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">await</span><span style="color:#24292E;"> app.</span><span style="color:#6F42C1;">RunOaktonCommands</span><span style="color:#24292E;">(args);</span></span></code></pre></div><h2 id="initialize-migration-infrastructure" tabindex="-1">Initialize Migration Infrastructure <a class="header-anchor" href="#initialize-migration-infrastructure" aria-label="Permalink to &quot;Initialize Migration Infrastructure&quot;">​</a></h2><p>Ok. Now that your project is properly configured, what&#39;s next? In your project directory, run the following command:</p><div class="language-bash vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">bash</span><pre class="shiki github-dark vp-code-dark"><code><span class="line"><span style="color:#B392F0;">sable</span><span style="color:#E1E4E8;"> </span><span style="color:#9ECBFF;">init</span><span style="color:#E1E4E8;"> </span><span style="color:#79B8FF;">--database</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">&lt;</span><span style="color:#9ECBFF;">database-nam</span><span style="color:#E1E4E8;">e</span><span style="color:#F97583;">&gt;</span><span style="color:#E1E4E8;"> </span><span style="color:#79B8FF;">--schema</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">&lt;</span><span style="color:#9ECBFF;">schema-nam</span><span style="color:#E1E4E8;">e</span><span style="color:#F97583;">&gt;</span></span></code></pre><pre class="shiki github-light vp-code-light"><code><span class="line"><span style="color:#6F42C1;">sable</span><span style="color:#24292E;"> </span><span style="color:#032F62;">init</span><span style="color:#24292E;"> </span><span style="color:#005CC5;">--database</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">&lt;</span><span style="color:#032F62;">database-nam</span><span style="color:#24292E;">e</span><span style="color:#D73A49;">&gt;</span><span style="color:#24292E;"> </span><span style="color:#005CC5;">--schema</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">&lt;</span><span style="color:#032F62;">schema-nam</span><span style="color:#24292E;">e</span><span style="color:#D73A49;">&gt;</span></span></code></pre></div><p>The default values for the database and schema are <code>Marten</code> and <code>public</code>, respectively. <code>Marten</code> is the name associated with the database for the default configuration. This is important when multiple databases are referenced in the same project.</p><p>Running the command above should have created some migration file in the <code>./sable/&lt;database-name&gt;/migrations</code> directory.</p><h2 id="backfill-initial-migrations" tabindex="-1">Backfill Initial Migrations <a class="header-anchor" href="#backfill-initial-migrations" aria-label="Permalink to &quot;Backfill Initial Migrations&quot;">​</a></h2><p>Once the migration infrastructure has been initialized for the Marten database, there&#39;s one more thing to do before proceeding. The Postgres databases are already up to date, so we must not apply the newly generated migrations. Instead, we&#39;ll just backfill them. <strong>Sable</strong> maintains a table to keep track of applied migrations in the database. Whenever a new migration is applied, <strong>Sable</strong> inserts a new record in that table for that migration to ensure it is applied only once. In our case, since the Postgres databases are already up to date, the newly generated migrations have already been applied without <strong>Sable</strong>, so we&#39;ll just backfill them:</p><div class="language-bash vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">bash</span><pre class="shiki github-dark vp-code-dark"><code><span class="line"><span style="color:#B392F0;">sable</span><span style="color:#E1E4E8;"> </span><span style="color:#9ECBFF;">migrations</span><span style="color:#E1E4E8;"> </span><span style="color:#9ECBFF;">backfill</span><span style="color:#E1E4E8;"> </span><span style="color:#79B8FF;">--database</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">&lt;</span><span style="color:#9ECBFF;">database-nam</span><span style="color:#E1E4E8;">e</span><span style="color:#F97583;">&gt;</span></span></code></pre><pre class="shiki github-light vp-code-light"><code><span class="line"><span style="color:#6F42C1;">sable</span><span style="color:#24292E;"> </span><span style="color:#032F62;">migrations</span><span style="color:#24292E;"> </span><span style="color:#032F62;">backfill</span><span style="color:#24292E;"> </span><span style="color:#005CC5;">--database</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">&lt;</span><span style="color:#032F62;">database-nam</span><span style="color:#24292E;">e</span><span style="color:#D73A49;">&gt;</span></span></code></pre></div><p>Running the command above should have created a new migration file in the <code>./sable/&lt;database-name&gt;</code> directory called <code>&lt;timestamap&gt;_backfill.sql</code>. Apply it to your database. That&#39;s all it takes to integrate <strong>Sable</strong> into an existing project. From this point on, treat the project as if it had been integrated with <strong>Sable</strong> from the very beginning.</p><p>To learn more about how <strong>Sable</strong> works, see <a href="./../reference/how-sable-works.html">How Sable Works</a>.</p>`,18),p=[e];function t(r,c,i,y,E,d){return a(),n("div",null,p)}const h=s(l,[["render",t]]);export{u as __pageData,h as default};
