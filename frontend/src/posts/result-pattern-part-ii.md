---
title: 'The Result Pattern: Part II - Pattern Matching'
date: 2025-09-05
excerpt: 'Part II of a three part series on the Result pattern'
tags:
  - rust
  - pattern matching
---

# Catching up

In [Part I](./result-pattern-part-i.md) we introduced the Result pattern and showed how it can help us be more expressive to help the _people_ working with our code to understand our intent.

In this Part II I'd like to show some other languages that handle this pattern in interesting ways.

# Let's get Rusty

_Rust_ is a pretty cool language. Among other things, it has some really nice _pattern matching_ features that let you be really expressive _and_ concise at the same time. These features are so entrenched in the language that the Result pattern is baked into the standard library.

## Enums

There is one important thing to understand before we start looking at some Rust code, especially if you come from a C-like or Java-like background like I do: enums exist in Rust, but they are not like enums in most other languages.

_Enum_ is short for _enumeration_, which according to Google, means "the action of a number of things one by one". _A number of things_ implies that the set of things has a limit, and this is what we usually mean with enums in code. We are restricting a value to a set number of possible values.

Rust takes enums to the next level, because they can be used as _union_ types.

Following our previous Divide example, where we can return one of a couple of different things, namely:

- an error saying we can't divide by zero
- an error saying we can't divide a number by itself
- the result of normal division

Rust lets us do something cool with this by letting us define it as an enum:

```rust
enum DivideResult {
    Ok(i32),
    DivideByZeroError,
    DivideBySelfError
}
```

See how it looks a little different from other enums? In Rust, our enums' _values_ can themselves be accompanied with other arbitrary data.

So an equivalent `Divide()` function in Rust could look a little something like this:

```rust
fn divide(num: i32, divisor: i32) -> DivideResult {
    if num == 0 {
        return DivideResult::DivideByZeroError;
    }
    if num == divisor {
        return DivideResult::DivideBySelfError;
    }

    return DivideResult::Ok(num / divisor);
}
```

This shouldn't look too scary once you get acquainted with the syntax. Got it? Cool.

# Patterns

Now that we know about enums, let's take a look at _pattern matching_.

Pattern matching is a paradigm where we evaluate some code against a pattern, and we can treat the thing we are evaluating differently based on whether the pattern matches or not.

Going again back to our Typescript example from Part I:

```typescript
function CallDivide(): number {
  let result = Divide(5, 0);

  if (result.ok) {
    return result.value;
  } else {
    return 0;
  }
}
```

Typescript actually has a kind of pattern matching: it calls them _type guards_. Recall that we needed to check the value of `result.ok` before Typescript would let us actually access `result.value`. This is an example of a type guard, once we know that `result.ok` is true, the only possible shape that the result could take is `Ok`, which has the `value` prop on it. Specifically, this is a _narrowing_ type guard, because we narrowed from a _possibility_ of `Ok | Err` to knowing for sure that we are actually an `Ok`.

Pattern matching is a way for us to tell the language we're working in to help us out by using narrowing. It's a little like Schrodinger's Cat. Before we look inside the box, we don't know if the cat is alive or not. Once we have looked in the box, the possibilities of the cat being alive or dead have collapsed into a single outcome, and we proceed with that outcome.

Now, let's get back to Rust.

Let's port our previous `CallDivide()` function to Rust:

```rust
fn CallDivide() -> number {
  let result = Divide(5, 0);

  if let DivideResult::Ok(val) == result {
    return val;
  } else if DivideResult::DivideByZero == result {
    return 0;
  } else if DivideResult::DivideBySelf == result {
    return 1;
  }
}
```

The thing that jumps out here is the `if let` syntax. It's like we're defining a variable in an if statement.

_That's exactly what we're doing!_

And that's exactly what we do with pattern matching.

We tell the language, please give me a variable for when this input matches this pattern. And then we use that variable to access the original input as if it matches the pattern!

Now let's turn it up to 11 with the `match` expression:

```rust
fn CallDivide() -> number {
  let result = Divide(5, 0);

  return result match {
    DivideResult::Ok(val) => val,
    DivideResult::DivideByZero => 0,
    DivideResult::DivideBySelf => 1,
  };
}
```

Look how we condensed those if statements away and how concise this code just became.

It also became _safer_, because if we change `Divide` to return something else, _this code breaks!_

Rust does _exhaustive_ pattern matching. It checks that you have handled every possible case, and it is a _compile time_ error if you haven't handled something.

So next time business adds a new rule to our `Divide` function, we know exactly where else we need to focus our attention, because the compiler tells us we haven't handled the new rule! No need for tests, no need for this to even leave the developer's machine!

It's like a switch statement that grew up and got it's life together.

And there are other languages that do it too: from a quick Google, [F#](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/match-expressions), [PHP](https://www.php.net/manual/en/control-structures.match.php), [Scala](https://docs.scala-lang.org/overviews/scala-book/match-expressions.html), [Python](https://peps.python.org/pep-0622/) all have pattern matching.

_Anyone else a little shocked that PHP is in the list?_

Even [C#](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/pattern-matching) has pattern matching as of C# 9.0. However, it has some drawbacks that I'll cover in Part III.
