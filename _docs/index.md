---
# https://vitepress.dev/reference/default-theme-home-page
layout: home

title: Sable
titleTemplate: Database Migrations for Marten

hero:
  name: Sable
  text: Database Migrations for Marten
  tagline: Simple, easy to use database migration management tool for Marten.
  image:
    src: /logo.svg
    alt: Sable logo
  actions:
    - theme: brand
      text: Get Started
      link: /introduction/getting-started
    - theme: alt
      text: Why Sable?
      link: /introduction/why-sable
    - theme: alt
      text: View on GitHub
      link: https://github.com/bloomberg/sable

features:
  - icon: 💡
    title: Intuitive
    details: Sable comes with an intuitive, easy to use CLI that is heavily inspired by other popular migration tools like dotnet-ef.
  - icon: 🚀
    title: Seamless Integration
    details: Sable can be seamlessly integrated into new, or existing projects. Integration is also minimally invasive. Opt-in or opt-out anytime with absolutely no headaches.
  - icon: 💪🏾
    title: Comprehensive Support
    details: Well-thought out to support any Marten configuration, including those that have multiple database references or multi-tenancy setups.
---
<style>
:root {
  --vp-home-hero-name-color: transparent;
  --vp-home-hero-name-background: -webkit-linear-gradient(120deg, #bd34fe 30%, #41d1ff);

  --vp-home-hero-image-background-image: linear-gradient(-45deg, #bd34fe 50%, #47caff 50%);
  --vp-home-hero-image-filter: blur(40px);
}

@media (min-width: 640px) {
  :root {
    --vp-home-hero-image-filter: blur(56px);
  }
}

@media (min-width: 960px) {
  :root {
    --vp-home-hero-image-filter: blur(72px);
  }
}
</style>
