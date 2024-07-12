import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  base: "/sable/",
  title: "Sable",
  description: "Database Migration Management for Marten",
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: 'Introduction', link: '/introduction/why-sable' }
    ],
    logo: '/logo.svg',
    sidebar: [
      {
        text: 'Introduction',
        items: [
          { text: 'Why Sable?', link: '/introduction/why-sable' },
          { text: 'Getting Started', link: '/introduction/getting-started' }
        ]
      },
      {
        text: 'Guide',
        items: [
          { text: 'Multi-Tenancy Setup', link: '/guide/multi-tenancy-setup' },
          { text: 'Multiple Database Setup', link: '/guide/multiple-database-setup' },
          { text: 'Existing Project Integration', link: '/guide/existing-project-integration' }
        ]
      },
      {
        text: 'Reference',
        items: [
          { text: 'Command Line Interface', link: '/reference/cli' },
          { text: 'How Sable Works', link: '/reference/how-sable-works' }
        ]
      }
    ],
    search: {
      provider: 'local'
    },
    socialLinks: [
      { icon: 'github', link: 'https://github.com/bloomberg/sable' }
    ]
  }
})
