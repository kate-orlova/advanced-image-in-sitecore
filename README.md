[![GitHub license](https://img.shields.io/github/license/kate-orlova/advanced-image-in-sitecore.svg)](https://github.com/kate-orlova/advanced-image-in-sitecore/blob/master/LICENSE)
![GitHub language count](https://img.shields.io/github/languages/count/kate-orlova/advanced-image-in-sitecore.svg?style=flat)
![GitHub top language](https://img.shields.io/github/languages/top/kate-orlova/advanced-image-in-sitecore.svg?style=flat)
![GitHub repo size](https://img.shields.io/github/repo-size/kate-orlova/advanced-image-in-sitecore.svg?style=flat)

# Advanced Image in Sitecore
The Advanced Image in Sitecore module helps to control the cropping of images with a focal point. One can choose an important aspect of an image that will serve as a central area around which the crop will happen. Such intelligent cropping of images allows to fine-tune the attractive scene of images for better visual impression.

The solution consists of the following key elements:
1. Admin interface
1. Processors
1. Requests
1. Pipelines
1. Front-end
1. Config
1. Sitecore package

## 1. Admin Interface
The main fields  in `\Fileds\` folder are
1. **Advanced Image** - a single image
1. **Advanced Image Gallery** - an image gallery
1. **Slider** - to manage the focal point for an image in Stecore interface

There are a few admin views in `\Views\Shared\` folder for
- image
- image details
- gallery
- gallery items
- thumbnails
- image update

Example of the editing interface for an image:

![Editing interface for an image](/assets/editing%20interface%20for%20image.png)


All admin CSS, JavaScript and templates are defined in `\sitecore modules\` folder.

Glass Mapper ORM is used to map the advanced image types from Sitecore field values to typed business entities, see `\GlassMapper\` folder.

## 2. Processors
`..\src\Foundation\AdvancedImage\Processors\CropProcessor.cs` is a cropping processor being called by Sitecore's Media Handler.

## 3. Requests
`..\src\Foundation\AdvancedImage\Requests\CropMediaRequest.cs` defines the cropping media request being called from the front-end and makes sure that the custom parameters are cached in `.ini` file within `/App_Data/` folder.

URL params are being specified  in `..\src\Foundation\AdvancedImage\GlassMapper\Fields\AdvancedImageField.cs`:
- CropX, CropY - for the cropping
- FocusX, FocusY - for the focal point
- ShowFull

## 4. Pipelines
`..\src\Foundation\AdvancedImage\Pipeline\AddItemLinkReferencesExtended.cs` is an extension for publishing pipeline for related items.

## 5. Front-end
There is an HTML extension `RenderImageLazy()` in `..\src\Foundation\AdvancedImage\Extensions\HtmlHelperExtensions.cs` rending an image as per the specified configuration accordingly. You can simply call it from your view as follows:
```
@Html.RenderImageLazy(YourModel.YourImage)
```
or
```
@Html.RenderImageLazy(x => x.YourImage, cropFactor: 1)
```

## 6 Config
`..\src\Foundation\AdvancedImage\App_Config\Include\Foundation.AdvancedImage.config` specifies the image processing pipeline.

`..\src\Foundation\AdvancedImage\App_Config\Include\Sitecore.Framework.Fields.AdvancedImage.config` defines the Advanced Image module settings in Sitecore.


## 7. Sitecore Package
There is a Sitecore package file `Sitecore package for AdvancedImage module.zip` in `..\Sitecore packages` folder, it is composed of the following sources:
1. Advanced Field Types
<img src="https://github.com/kate-orlova/advanced-image-in-sitecore/blob/master/assets/Sitecore%20package/Advanced%20Field%20Types.png" alt="Advanced Field Types" width="350">

2. Data Templates
<img src="https://github.com/kate-orlova/advanced-image-in-sitecore/blob/master/assets/Sitecore%20package/Data%20Templates.png" alt="Data Templates" width="350">

3. Module Thumbnails
<img src="https://github.com/kate-orlova/advanced-image-in-sitecore/blob/master/assets/Sitecore%20package/Module%20Thumbnails.png" alt="Module Thumbnails" width="350">

4. Custom Experience Buttons
<img src="https://github.com/kate-orlova/advanced-image-in-sitecore/blob/master/assets/Sitecore%20package/Custom%20Experience%20Buttons.png" alt="Custom Experience Buttons" width="350">

# Installation Guide
1. Add the **AdvancedImage** project to the **Foundation** layer of your Sitecore solution and build it;

1. Install the Sitecore package to have the required pre-requisite in place such as _Field Types, Data Templates_ and control elements;

Similarly you can configure an image gallery, its editing interface will look as follows:

![Editing interface for an image gallery](/assets/editing%20interface.png)

# Contribution
Hope you found this module useful, your contributions and suggestions will be very much appreciated. Please submit a pull request.

# License
The Advanced Image in Sitecore module is released under the MIT license what means that you can modify and use it how you want even for commercial use. Please give it a star if you like it and your experience was positive.
