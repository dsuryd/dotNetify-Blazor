## Scoped CSS
 
 Blazor native support for component-scoped CSS isn't here yet, but these steps will allow you to have it:

1. Create a separate css file for you component, e.g. _MyComponent.razor.css_.
2. Set the _Build Action_ property of that file to <b>Embedded Resource</b>.
3. Inside your component's razor file, inject **IStylesheet** service from _DotNetify.Blazor_ namespace.
4. Nest the component's HTML under a `d-panel` web component from `dotNetify-Elements`.
5. Use the indexer of _IStylesheet_ to access the css content of the file by its file name (it uses submatching, so name can be partial), and pass the string to the _css_ attribute of `d-panel`.

```jsx
@inject IStylesheet Stylesheet

<VMContext VM="HelloWorld">
  <d-panel css='@Stylesheet["HelloWorld"]'>
    @state.Greetings
  </d-panel>
</VMContext>
```

At runtime you will see that a CSS class with a random name is added to the `<head>` tag, and set on `d-panel` element, which makes the style exclusive to that element and its children. Furthermore, the light CSS preprocessor used to generate it supports nesting syntax. For example: `& { .some-class { &:hover { color: blue; } } }`.
