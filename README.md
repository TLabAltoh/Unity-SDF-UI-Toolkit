# Unity-SDF-UI-Toolkit

This plugin is based on [Unity-UI-Rounded-Corners](https://github.com/kirevdokimov/Unity-UI-Rounded-Corners) created by [kirevdokimov](https://github.com/kirevdokimov). UI components that render based on SDF. Rounded corners, outlines, SDFTexture editor. Additional shapes implemented based on [this article](https://iquilezles.org/articles/distfunctions2d/) (not all)

## Screenshot
<table>
    <caption>Overview</caption>
    <tr>
        <td><img src="Media/overview.png" width="512"></img>  </td>
    </tr>
</table>
<table>
    <caption>Editor for creating Custom Shapes</caption>
    <tr>
        <td><img src="Media/sdf-tex-painter-path-view.png" width="512"><img></td>
        <td><img src="Media/sdf-tex-painter-sdf-view.png" width="512"><img></td>
    </tr>
</table>

## Install
### Git
Clone this repository with the following command
```
git clone https://github.com/TLabAltoh/Unity-SDF-UI-Toolkit.git
```

or

```
git submodule add https://github.com/TLabAltoh/Unity-SDF-UI-Toolkit.git
```
### Unity Package Manager
add package from git URL ...
```
https://github.com/TLabAltoh/Unity-SDF-UI-Toolkit.git
```

## SDF Texture
Select ```Create/TLab/UI/SDF/SDF Tex Painter``` to add a Scriptable Object to the asset.

### Circle
|  |   |
| ------ | ------ |
| Mouse Drag | Move Handle |
| Shift + Left Click | Select Handle |
| Delete | Delete Selected Handle |

### Bezier
|  |   |
| ------ | ------ |
| Mouse Drag | Move Anchor Handle |
| Crtl + Mouse Drag | Move Control Handle |
| Shift + Left Click | Select Anchor Handle |
| Delete | Delete Selected Handle |

### Cu2Qu
SDF Text Painter converts the cubic Bezier curve to a quadratic Bezier curve based on [this code](https://github.com/googlefonts/cu2qu).

## Lisence
This repository is MIT licensed.
