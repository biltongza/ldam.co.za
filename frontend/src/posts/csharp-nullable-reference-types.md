---
title: 'C#: Nullable Reference Types (aka How The Fuck Was This Working Before?)'
date: 2024-04-06
excerpt: 'Experience in enabling nullable reference types with System.Text.Json'
tags:
  - c#
  - dotnet
  - swearing
---

# Reference types 101

Firstly, let's make sure we're all on the same page of what a reference type even is.

C# has two kinds of types: [Reference types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types) and [Value types](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types).

In short, reference types are `class`es. Anything that inherits from `System.Object` is a reference type.

Value types, on the other hand, are `struct`s. `int`, `bool`, `DateTime` are some common value types.

C# lets you assign `null` to variables of reference types. It does not allow you to assign `null` to variables of value types. Go ahead and try it:

```csharp
int a = 0; // works fine!
int b = null // CS0037 Cannot convert null to 'int' because it is a non-nullable value type

object x = new Object(); // works fine!
object y = null; // also works fine!
```

The reason for this, is that reference types are _actually_ just things that _point_ to where the (instance of the) data is actually stored, while a value type is _actually_ an instance of the data itself.

Without going into too much detail, this is kind of how C#, or rather the underlying runtime, can manage memory allocations for us.

# But reference types are already nullable?

That's what I thought when [nullable reference types](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references) were announced. What on earth do you mean I can _enable_ nulls? Literally a few sentences ago I said that C# lets you assign null to reference types. What gives?

It turns out, that nullable reference types are actually a compiler analysis feature, which lets it determine, and subsequently tell you, if you've done your due dilligence of checking for nulls.

_Personally, I think it's a poorly named feature. In fact, the concept of nulls are pretty crappy, especially compared to the [Option type of F#](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/options) or the [Option enum of Rust](https://doc.rust-lang.org/rust-by-example/std/option.html). But that's a story for another day._

## Let's look at an example

Consider the following code snippet:

```csharp
Person p = new Person();

Console.WriteLine(p.FirstName.ToUpper());

class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
```

Assuming you're using .NET 6 or later, this code will compile with zero warnings or errors. And yet, when you actually _run_ the program, it will explode with a `NullReferenceException`.

This is not news. We didn't assign the `FirstName` property, so it has its default value of `null` (as all reference types do).

Now let's enable nullable reference types and see what happens with the same code:

```csharp
#nullable enable
Person p = new Person();

Console.WriteLine(p.FirstName.ToUpper());

class Person
{
    public string FirstName { get; set; } // Non-nullable property 'FirstName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
    public string LastName { get; set; } // Non-nullable property 'LastName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
}
```

We have some new yellow squigglies in our editor which means the compiler is warning us about something. In fact, when you enable nullable reference types, you're going to see this warning _a lot_. Let's unpack it:

```plaintext
Non-nullable property 'FirstName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
```

Firstly, we are told which property has the problem, and we are told it is _non-nullable_. Strings are reference types, and that means we are allowed to assign null to them, _and_ their default value is null. Yet the compiler says non-nullable. This is what turning on nullable reference types does: _reference types are now implicitly non-nullable_.

Secondly, we are told the property must contain a non-null value when exiting the constructor.

_What the hell?_ We didn't write a constructor, so why are we being told about the constructor?

Well, it's because every class has a _default_ constructor that the compiler creates for you behind the scenes, since they are a necessary part of C# plumbing. Without a constructor, you could not use `new`. When you define a constructor, the compiler uses your one instead of making one for you.

This part of the warning is simply telling us that by the time _whichever_ constructor exits, the property value must not be null.

Finally, the last part: consider declaring the property as nullable. This is the escape hatch: if this property _can possibly contain null_, then we should indicate that by declaring it as nullable. We can do that by putting a `?` after the type, for example, `string?`.

Let's see what happens if we silence these warnings by declaring both properties as nullable.

```csharp
#nullable enable
Person p = new Person();

Console.WriteLine(p.FirstName.ToUpper()); // CS8602: Dereference of a possibly null reference.

class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
```

The yellow squigglies we had before are gone, but we have a new one where we access `FirstName`. Let's take a closer look at the warning:

```plaintext
Dereference of a possibly null reference.
```

What's a "dereference"? Think about the name _reference_ types. Remember that a reference type is literally a thing that _refers_ to some data. Calling `.ToUpper()` on a thing that _refers_ to something else does not make a lot of sense, and what we actually want to do is call that method on the thing that the reference is _referring to_. The act of accessing the data that a reference is referring to, is called _dereferencing_.

Now think about what would happen if we tried to find out what null is referring to. It makes no sense. Null does not refer to anything. And _this_ is the source of a `NullReferenceException`.

So basically, the compiler has figured out, that since `FirstName` has been marked as nullable, it is _possible_ that `FirstName` _might_ refer to null. It's telling us that we _might_ have a bug.

And it is right, in this case, because when we run the program, it explodes with a `NullReferenceException`. Good job compiler!

Now, how do we fix the bug? One easy solution is to check for null:

```csharp
#nullable enable
Person p = new Person();

if (p.FirstName != null)
{
    Console.WriteLine(p.FirstName.ToUpper());
}

class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
```

Now when we compile and run this, the warning is gone. The compiler is smart enough to realise that we added a check that `FirstName` is not null, so it can reasonably guarantee that when we dereference it when we call `.ToUpper()` on it, it will not be null.

And of course, we don't get an exception when we run this now.

# So they're just... warnings?

Yup. Pretty much. At the moment.

You can read the [proposal](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-8.0/nullable-reference-types.md#breaking-changes) for this feature where it indicates that this is a very obvious breaking change that needs to be introduced over time. So for now, these are just warnings.

However, they're also _not_ just warnings.

# Enter System.Text.Json

For a really long time, pretty much everyone that has needed to do anything JSON related in .NET has used [Json.NET, aka Newtonsoft.Json](https://www.newtonsoft.com/json). And for good reason too, it's really simple to get it up and running, so it became ubiquitous and has even been used by Microsoft themselves for multiple parts of the .NET ecosystem, including ASP.NET.

Then, in .NET Core 3.0, [System.Text.Json was announced](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-apis/). For a multitude of reasons, Microsoft decided that they needed to stop using Json.NET, and provide their own implementation of JSON processing.

System.Text.Json has evolved since then, and in .NET 6, Microsoft [introduced the ability to use _source generators_](https://devblogs.microsoft.com/dotnet/try-the-new-system-text-json-source-generator/) with System.Text.Json. The aim of this feature is to improve performance: usually you have to use runtime reflection to do JSON serialization and deserialization. While reflection is not the performance hog we were all warned about years ago, it does still have some consequences around memory allocation and startup time (at least these are the reasons cited by Microsoft in their post about the feature).

I am using this feature in this website: the photos you see on this site are all retrieved from Adobe Creative Cloud's APIs which are all JSON based, which I access using this source generator feature. Performance is not really a concern for me, as I'm not doing the millions and billions of requests that would benefit from any performance gains, I opted for it just because I wanted to learn how to use it.

## What does this have to do with nullable reference types?

Well, as I discovered today, a lot. Before I explain the link, I need to show you one more feature about nullable reference types: the `required` modifier, which was added in C# 11 aka .NET 7. [Documentation here](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required).

Let's go back to our original example:

```csharp
#nullable enable
Person p = new Person();

Console.WriteLine(p.FirstName.ToUpper());

class Person
{
    public string FirstName { get; set; } // Non-nullable property 'FirstName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
    public string LastName { get; set; } // Non-nullable property 'LastName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
}
```

There is another way to silence this warning: by marking the properties as `required`, like so:

```csharp
#nullable enable
Person p = new Person();

Console.WriteLine(p.FirstName.ToUpper());

class Person
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
```

By doing this, we are telling the compiler that for this class to be valid, the property _must_ be initialized. Now we have a **red** squigly, oh no! An error! Let's have a look at it:

```plaintext
error CS9035: Required member 'Person.FirstName' must be set in the object initializer or attribute constructor.
error CS9035: Required member 'Person.LastName' must be set in the object initializer or attribute constructor.
```

The compiler knows now that creating a `Person` without setting `FirstName` and `LastName` would be an error, because we told it that those properties are _required_. So it won't let us do that! We can fix this either by adding a constructor in which we set the properties, _or_ by assigning them with an object initializer, like so:

```csharp
Person p = new Person() { FirstName = "Logan", LastName = "Dam" };
```

## Are we there yet?

Finally, getting to the point. It turns out that System.Text.Json [supports this too](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/required-properties). System.Text.Json did not support the concept of _required properties_ at all before .NET 7. Now, it does it in a language integrated way, and initially, I'm quite a fan.

When I started enabling nullable reference types in my project, I actually wasn't expecting much, since I am _usually_ pretty good at remembering that nulls can happen, and my code base is small enough that any issues were not likely to be showstoppers. I started out with all my logic, since there wasn't too much of it. All went well.

Then I turned it on in the JSON [POCOs](https://en.wikipedia.org/wiki/Plain_old_CLR_object).

I expected some dragons there. I'm working with a [publicly documented API](https://developer.adobe.com/lightroom/lightroom-api-docs/api/), but at the time I built the thing, [Adobe's API specification was (and is still) invalid](https://github.com/AdobeDocs/lightroom-partner-apis/issues/159). So auto generating a client was out of the question, and so I built one myself.

It would appear that I made a series of mistakes while doing that, and enabling nullable reference types actually shone some light upon the issue.

Although the last time I had touched the code was over 3 years ago, I generally have a pretty good memory of how stuff works, especially stuff I built, and so I delved in and sprinkled in nullable annotations where I thought they made sense, and I also sprinkled in some `required` keywords where I _thought_ they made sense.

Surely an `Id` property is required, right? 🤡

Well, as I found out, _not always_, especially if your code makes wrong assumptions.

## (Rightfully deserved) Explosions

```plaintext
Exception: System.Text.Json.JsonException: JSON deserialization for type 'ldam.co.za.lib.Lightroom.Asset' was missing required properties, including the following: id, type, subtype, updated, created, links, payload
```

First, can we all just take a moment to appreciate how lovely this error message is? It's telling me exactly what is missing (although the use of the word _including_ hints that it might not be that simple), and it's telling me exactly what type it was trying to deserialize. And isn't it awesome that this is enforced at compile time _and_ at runtime?

Second, _what the fuck?_ I've been using this code for 3 years. Have I just never accessed any of these properties? Actually, I'm certain I've used the `links` one, because I [use that property to navigate the API's paging](https://github.com/biltongza/ldam.co.za/blob/f85c0394017e07fc555d46dcaf96d6601c755072/fnapp/Services/LightroomService.cs#L62C13-L62C69).

_And now you're telling me it's missing?_

Turns out, I messed up the structure of the API response for getting the assets within an album. But not enough for it to matter, apparently, because my code has been happily running for the past 3 years without issue.

**But I wouldn't have known there was an issue there at all had I not gone and experimented with nullable reference types.**

# Moral of the story

If you've made it this far, congratulations, go treat yourself to a milkshake, because I've been taking a long and winding route to get here and you must be thirsty by now.

I could talk about how good type safety is (naturally I am a big fan of Typescript) because they're an extra tool in your belt that help you avoid surprises. And that much is true, they help me prove ahead of time that my code is "safe".

But that is another topic for another day, and instead, I want to say this:

_Give nullables a shot._

You can turn them on and off as you please. Stick a `#nullable enable` at the top of a file, and boom, you get extra null checks. Don't want to deal with them anymore? `#nullable disable`. Or you can go the nuclear option like I did and throw `<Nullable>enable</Nullable>` in your project file and deal with the fallout all at once.

You might just find yourself exclaiming _"how the fuck is this working?"_ and that in and of itself is a fantastic exercise to a) learn more and b) find sneaky bugs. Also swearing is fun.
