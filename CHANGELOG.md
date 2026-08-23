# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and releases should follow [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- `PisPasep` value object with strict 11-ASCII-digit parsing, leading-zero preservation, ANS-documented modulo-11 check-digit validation, and `IParsable`/`ISpanParsable` support.
- `Nit` value object with strict 11-ASCII-digit structural parsing, leading-zero preservation, explicit format-only validation semantics, and `IParsable`/`ISpanParsable` support without conflating NIT with PIS/PASEP/NIS.
- `TelefoneBrasileiro` value object and `TipoTelefoneBrasileiro` discriminator for fields that accept either fixed-line or mobile Brazilian numbers, delegating parsing, formatting, E.164 representation, and numbering-plan validation to `LandlinePhone` and `MobilePhone`.
- `InscricaoEstadual` value object with mandatory `BrazilianState` context, explicit 27-UF structural strategy matrix, `ISENTO` rejection, state-aware equality, and documented format-only validation where no UF checksum is embedded.
- `PlacaVeiculo` value object and `PadraoPlacaVeiculo` discriminator for previous national and Mercosur/PIV plate sequence patterns, with ASCII uppercase canonicalization, previous-pattern formatting, and optional algorithmic previous-to-Mercosur sequence conversion.
- `Renavam` value object with strict current 11-digit parsing, leading-zero preservation, modulo-11 check-digit validation, structural-only semantics, and `IParsable`/`ISpanParsable` support.
- `ChavePix` value object and `TipoChavePix` discriminator for CPF, CNPJ, mobile phone, email, and random EVP Pix keys, reusing existing primitives and applying Pix-specific canonicalization without external DICT lookup.
- `CpfCnpj` value object and `TipoCpfCnpj` discriminator for fields that accept either CPF or CNPJ, delegating validation, canonicalization, formatting, equality, and alphanumeric CNPJ support to the existing primitives.
- `Email` value object with strict ASCII dot-atom local-part validation, IDN domain normalization to lowercase Punycode, conservative case semantics, syntax-only validation, and `IParsable`/`ISpanParsable` support.
- `Cep` value object with strict eight-ASCII-digit and canonical `00000-000` parsing, leading-zero preservation, canonical formatting, structural-only validation semantics, and `IParsable`/`ISpanParsable`/`IFormattable` support without runtime CEP lookup.
- `MobilePhone` value object with strict national, formatted, +55, and E.164 parsing, shared Anatel DDD validation, mandatory nine-digit `9XXXX-XXXX` subscriber rules, legacy eight-digit mobile rejection, canonical formatting, and `IParsable`/`ISpanParsable`/`IFormattable` support.
- `LandlinePhone` value object with strict national, formatted, +55, and E.164 parsing, centralized Anatel DDD validation, fixed-line subscriber range checks, rural `57` support, canonical formatting, and `IParsable`/`ISpanParsable`/`IFormattable` support.
- `Cnh` value object for the 11-digit CNH National Registration Number with strict ASCII-digit parsing, modulo-11 dual check-digit validation, the inter-DV discount rule, `IParsable`/`ISpanParsable` support, leading-zero preservation, and explicit separation from CNH mirror and RENACH identifiers.
- `Rg` value object and `BrazilianState` context with explicit legacy RG strategies for all 27 UFs, strict state-aware parsing, São Paulo SSP/IIRGD check-digit validation, format-only validation where no sufficiently reliable checksum source is available, and explicit distinction from CIN.
- `Cnpj` value object with strict numeric and alphanumeric parsing, 2026 Receita Federal ASCII-minus-48 modulo-11 validation, uppercase canonicalization, canonical formatting, `IParsable`/`ISpanParsable` support, and leading-zero preservation.
- `Cpf` value object with strict parsing, modulo-11 validation, canonical formatting, `IParsable`/`ISpanParsable` support, and consumer documentation.
- Primary GitHub Actions CI workflow with locked restore, Release build, tests, Cobertura coverage, NuGet packaging, package validation, and downloadable coverage/package artifacts.
- CodeQL security analysis for C# on pull requests, pushes to `main`, and a weekly scheduled scan using a reproducible manual .NET build.
- Dependency Review on pull requests to block newly introduced dependencies with high or critical known vulnerabilities.
- Release workflow with SemVer validation, NuGet.org Trusted Publishing through GitHub OIDC, symbol publishing, GitHub Release creation, and a package-identity guard that prevents publishing the source template placeholder while still allowing source-template GitHub Releases without package artifacts.
- Manual release flow through `workflow_dispatch` that validates `main`, rejects existing tags, builds/tests/packs/validates the package, and only then creates the requested Git tag at the exact validated commit SHA.
- Portable VS Code recommendations, workspace settings, and tasks for restore, build, test, coverage, and NuGet packaging.
- Maintenance-only GitHub repository administration baseline covering template status, Actions permissions, `main` ruleset checks, security features, and final v1.0 verification.
- Optional SonarQube Cloud analysis using a locally pinned SonarScanner for .NET, repository-secret opt-in, configurable repository coordinates, and Coverlet/OpenCover coverage import.
- Centralized SemVer versioning with base version `1.0.0`, tag-driven release overrides, packaged assembly metadata validation, and E2E stable/prerelease/mismatch checks.
- Maintenance-only release-publishing validation covering manual release request validation, tag/SHA guarantees, the `NUGET_USER` opt-in decision matrix, and generated-template behavior.
- Reproducible .NET SDK selection through `global.json`, SDK analyzer baseline validation, native SDK Package Validation, packaged README metadata, and a generated `SECURITY.md` policy.
- One-time GitHub Template Repository initializer that uses the real `dotnet new` engine, validates the generated repository, commits the canonical output, and removes bootstrap-only assets after successful initialization.
- Maintenance E2E validation for GitHub Template initialization parity against direct `dotnet new` output.

### Changed

- Hardened GitHub Actions permissions to job scope where applicable and pinned `NuGet/login` to the immutable v1.2.0 commit SHA used by the release workflow.
- Hardened all eligible GitHub Actions references with immutable commit SHAs and disabled credential persistence on read-only checkouts.
- Extended `.editorconfig` with production-scoped reliability/API-usage rules and low-noise performance rules while keeping `CA1859` as a suggestion.
- NuGet.org publication is explicitly opt-in through the `NUGET_USER` repository variable; when it is absent, empty, or whitespace-only, the release still creates its tag and GitHub Release without starting OIDC authentication or `dotnet nuget push`.
- GitHub Release creation is now independent from NuGet enablement; when NuGet publication is enabled, the GitHub Release still waits for a successful NuGet publication before it is created.
- README quick-start guidance now shows the full clone/install/generate flow, optional `-o` output usage, unambiguous uninstall commands, and the automated GitHub Template initialization path in Portuguese and English.
- GitHub Template initialization now uses a dedicated `INITIALIZE_REPOSITORY_TOKEN` secret with workflow-write permission for the self-removing push, while keeping the workflow `GITHUB_TOKEN` read-only.
