import{_ as s,o as n,c as a,Q as o}from"./chunks/framework.b8722102.js";const d=JSON.parse('{"title":"Multi-tenancy Setup","description":"","frontmatter":{},"headers":[],"relativePath":"guide/multi-tenancy-setup.md","filePath":"guide/multi-tenancy-setup.md"}'),l={name:"guide/multi-tenancy-setup.md"},p=o(`<h1 id="multi-tenancy-setup" tabindex="-1">Multi-tenancy Setup <a class="header-anchor" href="#multi-tenancy-setup" aria-label="Permalink to &quot;Multi-tenancy Setup&quot;">​</a></h1><p>So, you already went through the <a href="./../introduction/getting-started.html">Getting Started</a> guide, and may be wondering if there&#39;s any additional configuration needed for multi-tenancy setups. Given that all Postgres databases for the configured tenants must have identical structures, there&#39;s none. Everything you learned in the getting started guide still applies for any single database setup with multi-tenancy configured. As such, a multi-tenancy setup will look something like this:</p><div class="language-c# vp-adaptive-theme"><button title="Copy Code" class="copy"></button><span class="lang">c#</span><pre class="shiki github-dark vp-code-dark"><code><span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">Marten</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">Oakton</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">BloombergLP</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Sable</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Extensions</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">BloombergLP</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Sable</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Samples</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Core</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"><span style="color:#F97583;">using</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">Weasel</span><span style="color:#E1E4E8;">.</span><span style="color:#B392F0;">Core</span><span style="color:#E1E4E8;">;</span></span>
<span class="line"></span>
<span class="line"><span style="color:#F97583;">var</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">builder</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> WebApplication.</span><span style="color:#B392F0;">CreateBuilder</span><span style="color:#E1E4E8;">(args);</span></span>
<span class="line"><span style="color:#E1E4E8;">builder.Host.</span><span style="color:#B392F0;">ApplyOaktonExtensions</span><span style="color:#E1E4E8;">();</span></span>
<span class="line"></span>
<span class="line"><span style="color:#E1E4E8;">builder.Services.</span><span style="color:#B392F0;">AddMartenWithSableSupport</span><span style="color:#E1E4E8;">(</span><span style="color:#B392F0;">_</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=&gt;</span></span>
<span class="line"><span style="color:#E1E4E8;">{</span></span>
<span class="line"><span style="color:#E1E4E8;">    </span><span style="color:#F97583;">var</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">options</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">new</span><span style="color:#E1E4E8;"> </span><span style="color:#B392F0;">StoreOptions</span></span>
<span class="line"><span style="color:#E1E4E8;">    {</span></span>
<span class="line"><span style="color:#E1E4E8;">        DatabaseSchemaName </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> </span><span style="color:#9ECBFF;">&quot;books&quot;</span></span>
<span class="line"><span style="color:#E1E4E8;">    };</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.</span><span style="color:#B392F0;">Connection</span><span style="color:#E1E4E8;">(builder.Configuration[</span><span style="color:#9ECBFF;">&quot;Databases:Books:BasicTier&quot;</span><span style="color:#E1E4E8;">]);</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.</span><span style="color:#B392F0;">MultiTenantedDatabases</span><span style="color:#E1E4E8;">(</span><span style="color:#B392F0;">x</span><span style="color:#E1E4E8;"> </span><span style="color:#F97583;">=&gt;</span></span>
<span class="line"><span style="color:#E1E4E8;">    {</span></span>
<span class="line"><span style="color:#E1E4E8;">        x.</span><span style="color:#B392F0;">AddMultipleTenantDatabase</span><span style="color:#E1E4E8;">(builder.Configuration[</span><span style="color:#9ECBFF;">&quot;Databases:Books:GoldTier&quot;</span><span style="color:#E1E4E8;">], </span><span style="color:#9ECBFF;">&quot;books_gold&quot;</span><span style="color:#E1E4E8;">)</span></span>
<span class="line"><span style="color:#E1E4E8;">            .</span><span style="color:#B392F0;">ForTenants</span><span style="color:#E1E4E8;">(</span><span style="color:#9ECBFF;">&quot;gold1&quot;</span><span style="color:#E1E4E8;">, </span><span style="color:#9ECBFF;">&quot;gold2&quot;</span><span style="color:#E1E4E8;">);</span></span>
<span class="line"><span style="color:#E1E4E8;">        x.</span><span style="color:#B392F0;">AddSingleTenantDatabase</span><span style="color:#E1E4E8;">(builder.Configuration[</span><span style="color:#9ECBFF;">&quot;Databases:Books:SilverTier&quot;</span><span style="color:#E1E4E8;">], </span><span style="color:#9ECBFF;">&quot;books_silver&quot;</span><span style="color:#E1E4E8;">);</span></span>
<span class="line"><span style="color:#E1E4E8;">    });</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.AutoCreateSchemaObjects </span><span style="color:#F97583;">=</span><span style="color:#E1E4E8;"> AutoCreate.None;</span></span>
<span class="line"><span style="color:#E1E4E8;">    options.Schema.</span><span style="color:#B392F0;">For</span><span style="color:#E1E4E8;">&lt;</span><span style="color:#B392F0;">Book</span><span style="color:#E1E4E8;">&gt;();</span></span>
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
<span class="line"></span>
<span class="line"><span style="color:#24292E;">builder.Services.</span><span style="color:#6F42C1;">AddMartenWithSableSupport</span><span style="color:#24292E;">(</span><span style="color:#6F42C1;">_</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=&gt;</span></span>
<span class="line"><span style="color:#24292E;">{</span></span>
<span class="line"><span style="color:#24292E;">    </span><span style="color:#D73A49;">var</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">options</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">new</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">StoreOptions</span></span>
<span class="line"><span style="color:#24292E;">    {</span></span>
<span class="line"><span style="color:#24292E;">        DatabaseSchemaName </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> </span><span style="color:#032F62;">&quot;books&quot;</span></span>
<span class="line"><span style="color:#24292E;">    };</span></span>
<span class="line"><span style="color:#24292E;">    options.</span><span style="color:#6F42C1;">Connection</span><span style="color:#24292E;">(builder.Configuration[</span><span style="color:#032F62;">&quot;Databases:Books:BasicTier&quot;</span><span style="color:#24292E;">]);</span></span>
<span class="line"><span style="color:#24292E;">    options.</span><span style="color:#6F42C1;">MultiTenantedDatabases</span><span style="color:#24292E;">(</span><span style="color:#6F42C1;">x</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=&gt;</span></span>
<span class="line"><span style="color:#24292E;">    {</span></span>
<span class="line"><span style="color:#24292E;">        x.</span><span style="color:#6F42C1;">AddMultipleTenantDatabase</span><span style="color:#24292E;">(builder.Configuration[</span><span style="color:#032F62;">&quot;Databases:Books:GoldTier&quot;</span><span style="color:#24292E;">], </span><span style="color:#032F62;">&quot;books_gold&quot;</span><span style="color:#24292E;">)</span></span>
<span class="line"><span style="color:#24292E;">            .</span><span style="color:#6F42C1;">ForTenants</span><span style="color:#24292E;">(</span><span style="color:#032F62;">&quot;gold1&quot;</span><span style="color:#24292E;">, </span><span style="color:#032F62;">&quot;gold2&quot;</span><span style="color:#24292E;">);</span></span>
<span class="line"><span style="color:#24292E;">        x.</span><span style="color:#6F42C1;">AddSingleTenantDatabase</span><span style="color:#24292E;">(builder.Configuration[</span><span style="color:#032F62;">&quot;Databases:Books:SilverTier&quot;</span><span style="color:#24292E;">], </span><span style="color:#032F62;">&quot;books_silver&quot;</span><span style="color:#24292E;">);</span></span>
<span class="line"><span style="color:#24292E;">    });</span></span>
<span class="line"><span style="color:#24292E;">    options.AutoCreateSchemaObjects </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> AutoCreate.None;</span></span>
<span class="line"><span style="color:#24292E;">    options.Schema.</span><span style="color:#6F42C1;">For</span><span style="color:#24292E;">&lt;</span><span style="color:#6F42C1;">Book</span><span style="color:#24292E;">&gt;();</span></span>
<span class="line"><span style="color:#24292E;">    </span><span style="color:#D73A49;">return</span><span style="color:#24292E;"> options;</span></span>
<span class="line"><span style="color:#24292E;">});</span></span>
<span class="line"></span>
<span class="line"><span style="color:#D73A49;">var</span><span style="color:#24292E;"> </span><span style="color:#6F42C1;">app</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">=</span><span style="color:#24292E;"> builder.</span><span style="color:#6F42C1;">Build</span><span style="color:#24292E;">();</span></span>
<span class="line"><span style="color:#24292E;">app.</span><span style="color:#6F42C1;">MapGet</span><span style="color:#24292E;">(</span><span style="color:#032F62;">&quot;/&quot;</span><span style="color:#24292E;">, () </span><span style="color:#D73A49;">=&gt;</span><span style="color:#24292E;"> </span><span style="color:#032F62;">&quot;💪🏾&quot;</span><span style="color:#24292E;">);</span></span>
<span class="line"></span>
<span class="line"><span style="color:#D73A49;">return</span><span style="color:#24292E;"> </span><span style="color:#D73A49;">await</span><span style="color:#24292E;"> app.</span><span style="color:#6F42C1;">RunOaktonCommands</span><span style="color:#24292E;">(args);</span></span></code></pre></div><p>Again, given that all Postgres databases for the configured tenants must have identical structures, there&#39;s no need to generate a different set of migrations for every single database identifier. So, for the example shown above, you just need to specify the database name as <code>Marten</code>, not <code>books_basic</code> nor <code>books_basic</code>, when running <strong>Sable</strong> commands. That&#39;s it.</p><p>See <a href="https://martendb.io/configuration/multitenancy.html" target="_blank" rel="noreferrer">Multi-tencancy Sample</a> for a sample application with a multi-tenancy setup.</p><p>To learn more about multi-tenancy in Marten, see <a href="https://martendb.io/configuration/multitenancy.html" target="_blank" rel="noreferrer">Marten Multi-tencancy</a>.</p>`,6),e=[p];function t(r,c,E,y,i,u){return n(),a("div",null,e)}const g=s(l,[["render",t]]);export{d as __pageData,g as default};
