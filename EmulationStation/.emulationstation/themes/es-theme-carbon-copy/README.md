# Carbon Copy

A fork of the EmulationStation theme "Carbon".

## Description

This is my customized fork of the [Fabrice CARUSO Carbon theme fork](https://github.com/fabricecaruso/es-theme-carbon).

Note that I:

* Use `US` as the theme region and don't fix/test other region settings.
* Only use ES for arcade, consoles and Steam - so changes for other system types like computers likely will not appear here.
* Will probably not keep this as updated as I'd like, or updated with the upstream repo, though I may PR some changes if they're appropriate.

## Notable Changes

The following are notable changes:

### Additional Platforms

The following platform art has been added:

* `megadrive-japan` - This is a commonly used "extra" platform to separate Japanese MD games
* `pspminis` - For PlayStation Minis, a.k.a. PSP Go digital games
* `xbla` - Dedicated platform for XBOX Live Arcade

### Updated Artwork

Filling in some blanks in the artwork and fixing some display issues, like the base Carbon theme (US region) displaying a TurboGrafx 16 console for the `pcengine` system, which it obviously isn't.

### Better Vertical Layout

Updates have been made for better content layout on vertical setups.

As a dev note, you use the `ifSubset="systemview:vertical"` attribute on display elements, or `verticalScreen="true"` attribute on settings, to conditionally display content for vertical setups.

```xml
<image name="box" extra="true" ifSubset="systemview:vertical">
    <pos verticalScreen="true">0.5 0.22</pos>
    <maxSize verticalScreen="true">0.33 0.33</maxSize>
    <visible verticalScreen="true">false</visible>
</image>
```

`ifSubset="previewbar:no|yes|vertical"`