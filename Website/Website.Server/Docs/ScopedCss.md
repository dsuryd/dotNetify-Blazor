## Scoped CSS

Blazor has no native support for component-scoped CSS yet, and so dotNetify provides the **StyleSheet** component to fill the gap. It works by looking for an embedded resource by the same name of the component where it is declared (or can be explicitly provided), loads the content as string and provides it to a custom web component called `d-style` which will apply the style sheet only to the DOM elements nested within it.

#### Basic Usage

To create a style sheet for a component, first add a new file to contain the style sheet and name it the same as your component's name with a `css` or 'scss` extension. For example: _MyComponent.razor.scss\_\_. Make sure to set the build property of the file to \_Embedded Resource_. Then in your component, declare this:

```jsx
<StyleSheet Context='this'>...</StyleSheet>
```

The **Context** attribute set to `this` will tell the component to find the resource matching the type name. If you want to provide a different name, use the **Name** attribute instead.

#### Style Sheet Syntax

The component uses a light CSS preprocessor to build style sheet, which allows SASS-like nesting capabilities such as this:

```jsx
& {
   .button {
     &:hover {
       background-color: #eee;
     }
   }
}
```

#### Parameterized Style Sheet

If you ever have the need to replace certain styles in the style sheet at runtime, the component provides the **OnLoad** event to allow the opportunity to modify the string content. You could insert a token in the style sheet, and replace it with an actual style on the component's load event:

```jsx
<StyleSheet Name="MyComponent" OnLoad="ChangeColor">
   ...
</StyleSheet>

@code {
  private string ChangeColor(string css)
  {
    return css.Replace("$color-to-replace", "red");
  }
}
```

#### Usage with Web Components

Most of _Element_'s web components accept a style sheet string through their `css` attribute. If you want to get a style sheet from an embedded resource and provide the string straight to this attribute, use the **IStyleSheet** service:

```jsx
@inject IStyleSheet StyleSheet

<d-panel css="@StyleSheet["MyComponent"]">
...
</d-panel>
```
