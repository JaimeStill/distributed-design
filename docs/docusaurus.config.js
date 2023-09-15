// @ts-check
// Note: type annotations allow type checking and IDEs autocompletion

const lightCodeTheme = require('prism-react-renderer/themes/github');
const darkCodeTheme = require('prism-react-renderer/themes/dracula');

/** @type {import('@docusaurus/types').Config} */
const config = {
  title: 'Distributed Design',
  tagline: 'Minimal dependency microservice architecture for .NET',
  favicon: 'img/favicon.ico',
  url: 'https://jaimestill.github.io',
  baseUrl: '/distributed-design',
  organizationName: 'JaimeStill',
  projectName: 'distributd-design',

  onBrokenLinks: 'throw',
  onBrokenMarkdownLinks: 'warn',

  i18n: {
    defaultLocale: 'en',
    locales: ['en'],
  },

  presets: [
    [
      'classic',
      /** @type {import('@docusaurus/preset-classic').Options} */
      ({
        docs: {
          routeBasePath: '/',
          sidebarPath: require.resolve('./sidebars.js')
        },
        blog: false,
        theme: {
          customCss: require.resolve('./src/css/custom.css'),
        },
      }),
    ],
  ],

  themeConfig:
    /** @type {import('@docusaurus/preset-classic').ThemeConfig} */
    ({
      image: 'img/social.png',
      navbar: {
        title: 'Distributed Design',
        logo: {
          alt: 'Distributed Design',
          src: 'img/logo.png',
        },
        items: [
          {
            type: 'docSidebar',
            sidebarId: 'docsSidebar',
            position: 'left',
            label: 'Docs',
          },
          {
            href: 'https://github.com/JaimeStill/distributed-design',
            label: 'GitHub',
            position: 'right',
          },
        ],
      },
      colorMode: {
        respectPrefersColorScheme: true
      },
      prism: {
        theme: lightCodeTheme,
        darkTheme: darkCodeTheme,
        additionalLanguages: ['csharp', 'powershell']
      },
    }),
};

module.exports = config;
