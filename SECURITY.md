# Security Policy

TeumesseCastle is an Active Directory security audit tool. It is typically run with privileged accounts on
sensitive infrastructure, so we take vulnerability reports seriously.

## Supported versions

| Version | Supported |
|---|---|
| 1.x (latest release) | Yes |
| anything older | No |

## Reporting a vulnerability

**Please do not open a public issue for security problems.**

Use GitHub private vulnerability reporting:
https://github.com/Teumesse0/TeumesseCastle/security/advisories/new

<!-- TODO(maintainer): add a dedicated security contact e-mail address once defined. -->

Include, when possible: the version (output of `--version`), the command line used, the environment
(OS, .NET Framework version, domain functional level), a description of the impact, and a proof of concept.
Never include real Active Directory data (domain names, SIDs, account names, hashes) in a report; anonymise
or synthesise it first.

## What to expect

- Acknowledgement within 7 days.
- An assessment and, if confirmed, a fix plan within 30 days.
- Coordinated disclosure: we ask reporters to keep the details private for **90 days** after the first
  acknowledgement, or until a fixed release is published, whichever comes first. We credit reporters in
  the release notes unless they prefer to stay anonymous.

## Scope and rules of engagement

- Only test TeumesseCastle against Active Directory environments **you own or are explicitly authorised to
  audit**. Running this tool against a third-party domain without written authorisation is illegal in most
  jurisdictions and is not covered by this policy.
- Findings about the *security posture of a domain* reported by the tool are not vulnerabilities in
  TeumesseCastle; they are the tool's purpose.
- Vulnerabilities in the original PingCastle code base that also affect TeumesseCastle are in scope for this
  project. We do not forward reports to the upstream vendor; TeumesseCastle is not affiliated with it.

## Network behaviour

TeumesseCastle never sends telemetry. The complete list of network connections it can open is documented in
`docs/NETWORK.md`.
