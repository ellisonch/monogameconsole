This project offers a developer console for MonoGame games.
It can be easily added to any MonoGame game and modified.

# Summary
```
GameConsole console = new GameConsole(this, spriteBatch); // where `this` is your `Game` class
```

By default, the console is opened with the <kbd>`</kbd> (OemTilde key) but this, amongst other settings, can be changed via the Options property.

Commands can be added using the `IConsoleCommand` interface.

# History
This project was ported from XNA console versions, which can be found here:
* https://github.com/dreasgrech/xnagameconsole
* https://github.com/jameswalkoski/jameswalkoski-xnagameconsole-xna4

# License
This project is dual-licensed under Apache 2.0 and MIT terms.