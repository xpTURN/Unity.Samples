# CsPreview

A **sample project** for integrating and trying out open-source projects in the Unity engine.

---

## Adopted Open-Source Projects

### Development Environment & Tools

| Project | Description | Installation | GitHub |
|---------|-------------|--------------|--------|
| **Unity IDE Cursor** | Unity integration for Cursor IDE | UPM | [boxqkrtm/com.unity.ide.cursor](https://github.com/boxqkrtm/com.unity.ide.cursor) |
| **NuGetForUnity** | Use NuGet packages in Unity | UPM | [GlitchEnzo/NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity) |

### C# 9·10·11 Extensions

| Project | Description | Installation | GitHub |
|---------|-------------|--------------|--------|
| **Polyfill** | C# Polyfill | UPM | [xpTURN/Polyfill](https://github.com/xpTURN/Polyfill) |

### Async·Performance·Strings

| Project | Description | Installation | GitHub |
|---------|-------------|--------------|--------|
| **UniTask** | async/await for Unity (replacement for C# Task) | UPM | [Cysharp/UniTask](https://github.com/Cysharp/UniTask) |

### Logging·Strings

| Project | Description | Installation | GitHub |
|---------|-------------|--------------|--------|
| **ZString** | Zero-allocation strings | UPM | [Cysharp/ZString](https://github.com/Cysharp/ZString) |
| **XString** | ZString wrapper/utilities | UPM | [xpTURN/XString](https://github.com/xpTURN/XString) |
| **ZLogger** | High-performance logging | NuGet + UPM | [Cysharp/ZLogger](https://github.com/Cysharp/ZLogger) |
| **XLogger** | ZLogger wrapper/utilities | UPM | [xpTURN/XLogger](https://github.com/xpTURN/XLogger) |

### DI·Routing

| Project | Description | Installation | GitHub |
|---------|-------------|--------------|--------|
| **VContainer** | DI for Unity | UPM + manual install | [hadashiA/VContainer](https://github.com/hadashiA/VContainer) |
| **VitalRouter** | Message router for Unity | NuGet + UPM | [hadashiA/VitalRouter](https://github.com/hadashiA/VitalRouter) |

### References

| Project | Description | Installation | GitHub |
|---------|-------------|--------------|--------|
| **AssetLink** | Asset reference/link utilities | UPM | [xpTURN/AssetLink](https://github.com/xpTURN/AssetLink) |
| **WeakRef** | Weak reference utilities | UPM | [xpTURN/WeakRef](https://github.com/xpTURN/WeakRef) |

---

## Package Adoption

- **Unity Package Manager (UPM)**: Packages registered via Git URL in `Packages/manifest.json`.
- **NuGet**: Packages installed via NuGetForUnity.
- **VContainer**: Runtime via UPM; source generator may be included as VContainer.SourceGenerator DLL in `Assets/Plugins`.

## Note

This repository is a sample for checking usage examples and combinations of the above libraries.
