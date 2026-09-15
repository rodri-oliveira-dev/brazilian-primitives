# Security Policy

## Supported Versions

Security fixes are prioritized for the latest released version and the current `main` branch.

Older versions may receive fixes when the impact is high and a backport is practical, but long-term support is not guaranteed by default.

## Reporting a Vulnerability

Please privately report suspected vulnerabilities instead of opening a public issue.

Preferred reporting channels are:

- GitHub private vulnerability reporting or Security Advisories, when enabled for the repository;
- a maintainer contact channel documented by the project, when private GitHub reporting is not available.

Include enough detail to help maintainers reproduce and assess the issue:

- affected package version or commit;
- affected platform or runtime, when relevant;
- a minimal reproduction or proof of concept;
- expected impact and any known mitigations.

## Automated Security and Dependency Maintenance

The repository uses layered automated controls rather than relying on a single scanner:

- Dependabot maintains NuGet packages, the .NET SDK declared by `global.json`, and GitHub Actions references;
- CodeQL performs C# static security analysis;
- Dependency Review evaluates dependency changes introduced by pull requests;
- CI and SonarQube Cloud provide complementary build, test, quality, and analysis gates.

.NET SDK updates are intentionally proposed as dedicated Dependabot pull requests so toolchain changes remain explicit and reviewable. Major SDK changes must be evaluated for compatibility before merge; the repository's `global.json` roll-forward and prerelease policy remains authoritative.

## Triage Expectations

Maintainers should acknowledge and triage reports as soon as reasonably possible. Response and fix timelines depend on severity, maintainer availability, release complexity, and coordinated disclosure needs.

Sensitive details should remain private until a fix or mitigation is available, unless disclosure is legally required or already public.
