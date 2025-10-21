# mk.helpers Documentation

This folder contains the DocFX configuration for generating API documentation.

## Prerequisites

Install DocFX:
```bash
dotnet tool install -g docfx
```

## Build Documentation Locally

1. Build the main project to generate XML documentation:
```bash
dotnet build ../mk.helpers/mk.helpers.csproj --configuration Release
```

2. Generate API metadata and build the site:
```bash
cd docs
docfx metadata
docfx build
```

3. Serve locally and preview:
```bash
docfx serve _site
```

Then open http://localhost:8080 in your browser.

## Auto-generated Documentation

Documentation is automatically built and published to GitHub Pages on every push to the main branch.

Visit: https://mcknight89.github.io/mk.helpers

## Configuration

- `docfx.json` - Main DocFX configuration
- `index.md` - Landing page
- `toc.yml` - Table of contents
- `api/` - Generated API documentation (gitignored)
- `_site/` - Generated static site (gitignored)

## Adding Custom Articles

Create markdown files in an `articles/` folder and reference them in `toc.yml`.
