---
title: 'The Result Pattern: Part I - Introduction'
date: 2025-09-04
excerpt: 'Part I of a two part series on the Result pattern'
tags:
  - javascript
  - patterns
---

# What's the Result pattern?

Let's start with the basics. What _is_ the Result pattern, and why should you care?

Put simply, the Result pattern is a neat way to tell people what to expect to get back when they call your code. It's a little bit of a contract.

Let's start with an example in everyone's favourite toy language: _javascript_.

Suppose we need to perform an operation, but we have a constraint on the inputs of the operation. How should we tell the user about these constraints?

Since we're in javascript, we have a couple of options.

We could just return a string and make it the caller's problem to figure it out:

```javascript
function Divide(num, divisor) {
  if (divisor === 0) {
    // how do we tell the caller we can't divide by zero?
    return "Can't divide by zero!";
  }

  return num / divisor;
}

var quotient = Divide(5, 0);

quotient += 3; // uh oh, now we have "Can't divide by zero!3"
```

The caller needs some divine knowledge to think, "wait a moment, maybe Divide returns string sometimes, so I need to check for that." Otherwise they might only see this happen at runtime when some poor sod tries to run their code for the first time.

That sucks a bit, so do we have any other options? Yes, of course, we can throw.

```javascript
function Divide(num, divisor) {
  if (divisor === 0) {
    throw "Can't divide by zero!";
  }

  return num / divisor;
}

var quotient = Divide(5, 0); // Uncaught Can't divide by zero!

quotient += 3; // this line never executes
```

In this case, the caller needs to remember to wrap this in a try/catch block. Anyone who's worked more than a month in enterprise has seen what that turns into:

```javascript
function CallDivide() {
  try {
    var quotient = Divide(5, 0);
    return quotient + 3;
  } catch (e) {
    LogError(e);
    return undefined;
  }
}
```

_\*Shudders\*_

Now what happens if this error is actually a normal part of the application flow, but it is not terminal, and we just need to do something a little differently? Following our example, maybe if we try to divide by zero, we just want to return zero instead:

```javascript
function CallDivide() {
  try {
    var quotient = Divide(5, 0);
    return quotient + 3;
  } catch (e) {
    return 0;
  }
}
```

Looks good. In all likelihood, this sort of code is probably running in some mission critical piece of software somewhere, and it's doing just fine there.

The problem only really shows up when the code starts changing.

We've all been there. We've inherited a codebase that was written by a team that has been replaced twice since you joined. Or you're building something new so you're not sure exactly what the real version will look like, and churn is frequent while you iterate on it.

Maybe you're just fixing a bug in production. Maybe there's a new rule the business wants to implement.

Actually, let's go with that example. Business now says, we can't divide if the number and the divisor are equal.
_No, that rule doesn't make sense, but when have you ever got a changing requirement that made perfect sense upfront?_

Let's update our `Divide()` function.

```javascript
function Divide(num, divisor) {
  if (divisor === 0) {
    throw "Can't divide by zero!";
  }

  if (num === divisor) {
    throw "Can't divide when number and divisor are equal!";
  }

  return num / divisor;
}
```

Perfect. The code now applies the business rule perfectly, we can't divide if the number and the divisor are equal.

We ship the code.

What's this? We're getting bug reports. People are seeing zeroes on their dashboards. Where did the bug come from? It can't be the new rule, that's got nothing to do with zeroes!

Well, let's look again at our `CallDivide()` function:

```javascript
function CallDivide() {
  try {
    var quotient = Divide(5, 0);
    return quotient + 3;
  } catch (e) {
    return 0;
  }
}
```

See the problem yet? We have't updated the code that calls our `Divide()` function to handle the new rule. We need to check the type of error to see if we should return zero now. And we didn't catch the bug before release because we don't have tests!

# Change is hard

It sounds cliche, because it is. Had the rules not changed, the code we had would have continued to work _just fine_ for as long as we let it. We know it would have, because that's how production works.

But the rules _will_ change.

We know this. We all experience it. We all have opinions about it.

But we're also _programmers_.

We can do something about this problem. We solve problems. We automate things.

We could write a test for this to make sure, _if it happens again_, we get a heads up this time.

We could put it in our _way of working_ documentation that _all edge cases must be handled_.

We could go rewrite the code to not call the `Divide()` function when the number and the divisor are equal (and now we have the same rule implemented differently in 2 places!).

All of these options have probably been tried, and will probably continue to be used in future.

But let me ask you a question, _what if we just never let the code with the bug in it run in the first place?_

Unfortunately, since we used javascript, that's not really an option (unless we start using Typescript, and I'm not here to start a holy war).

_(but for real you should just use Typescript)_

# Get to the point

Alright, now that we're all familiar with the problem, I can show you a new tool to use. The _Result pattern_.

Remember what I said in the beginning:

> Put simply, the Result pattern is a neat way to tell people what to expect to get back when they call your code.

Telling people what to _expect_ is informing them about our _result_.

And before the Java people get excited, yes, Java does kind of have this, because it forces you to declare what kinds of _exceptions_ you throw before you throw them:

```java
public int ThrowStuff(int arg) throws IllegalArgumentException {
    if(arg == 3) {
        throw new IllegalArgumentException("Can't call ThrowStuff with the number 3!");
    }

    throw new NullPointerException(); // illegal since I didn't include this exception type in the throws list
}
```

And before the Java people get _smug_, does anyone want to take a guess at why this sucks?

...

Because if you depend on it, you're depending on _exceptional_ behaviour. It's in the name. Exceptions break normal control flow. They are thrown in _exceptional_ cases. What if the _exception_ is actually part of the normal flow? We have a word for that: _swallowing_.

We should save exceptions for _truly_ exceptional behaviour. Where our normal program flow _should_ be interrupted. Where we _cannot_ recover from the scenario and we need to bail out.

So with that in mind, what if we had a way to tell the people that call our code, "normally we return A, sometimes we return B, and you need to handle both these cases".

What if we had a way to tell the people that call our code, "I'm going to validate your input, and if it's wrong, I'll tell you nicely because a human probably entered this and you might want to let them know".

Note what I said there.

Tell the _people_ that call our code.

_Remember earlier who cried earlier when the rule changed? Was it the code? No. It was the people._

# Just show me the thing already!

Okay, since you asked so nicely, but we're going to need to go into Typescript for this.

Keeping with our division example, an example of the result pattern might look a little something like this:

```typescript
type Ok<T> = {
  ok: true;
  value?: T;
};

type Err<E> = {
  ok: false;
  error: E;
};

type Result<T, E> = Ok<T> | Err<E>;

function Ok<T>(val: T): Ok<T> {
  return {
    ok: true,
    value: val
  };
}

function Err<E>(error: E): Err<E> {
  return {
    ok: false,
    error: error
  };
}

function Divide(num: number, divisor: number): Result<number, string> {
  if (num === 0) {
    return Err("Can't divide by zero!");
  }

  return Ok(num / divisor);
}
```

What are we looking at? Let's take it one piece at a time:

```typescript
type Ok<T> = {
  ok: true;
  value?: T;
};
```

Here we're telling the type system that we have a type called `Ok`, and it takes a generic type argument `T` that tells us what the type of `value` is.

```typescript
type Err<E> = {
  ok: false;
  error: E;
};
```

Here we're telling the type system that we have a type called `Err`, and it takes a generic type argument `E` that tells us what the type of `error` is.

Both of these types have a field in common: the `ok` field.

```typescript
type Result<T, E> = Ok<T> | Err<E>;
```

Here we tell the type system about our `Result` type. We say we can have a `Result` that either contains a value of type `T` if `ok` is true, or an error of type `E` if `ok` is false.

Then we have some little helper functions to make using these types a little less verbose:

```typescript
function Ok<T>(val: T): Ok<T> {
  return {
    ok: true,
    value: val
  };
}

function Err<E>(error: E): Err<E> {
  return {
    ok: false,
    error: error
  };
}
```

And finally, we have our `Divide()` function:

```typescript
function Divide(num: number, divisor: number): Result<number, string> {
  if (num === 0) {
    return Err("Can't divide by zero!");
  }

  return Ok(num / divisor);
}
```

It looks pretty much identical to before, doesn't it? So what difference does this make?

Well, for one, how we handle the value we get out of it now looks a little different:

```typescript
function CallDivide(): number {
  let result = Divide(5, 0);

  return result.value; // ts(2339): Property 'value' does not exist on type 'Result<number, string>'.
  //                                 Property 'value' does not exist on type 'Err<string>'.
}
```

Now if we try to access `value` directly on the result, we get an error! We don't even get to see what happens to this code at runtime, since we got stopped from running it in the first place.

We are now _forced_ to check if the result is ok or not:

```typescript
function CallDivide(): number {
  let result = Divide(5, 0);

  if (result.ok) {
    // this is now legal since we checked if `ok` is true,
    // and since `ok` can only be true on Ok results
    // we know `value` is safe to access.
    return result.value;
  } else {
    return 0;
  }
}
```

Cool. We got our compiler to help us enforce a constraint by making sure our code works with the constraint.

However, do we still have the same problem as before, where if `Divide()` introduces a new rule, does the calling code handle it?

The answer is *it depends*™️.

It depends on how we choose to introduce the new rule.

```typescript
function Divide(num: number, divisor: number): Result<number, string> {
  if (num === 0) {
    return Err("Can't divide by zero!");
  }

  if (num === divisor) {
    return Err("Can't divide when number and divisor are equal!");
  }

  return Ok(num / divisor);
}
```

If we did it like this, then _yes_, we would have exactly the same problem, because we haven't told our system that we actually have _different_ kinds of errors.

But we are _programmers_. We can solve this problem.

What if we instead handled it like this:

```typescript
enum DivisionErrors {
  DivideByZero,
  NumberAndDivisorEqual
}

function Divide(num: number, divisor: number): Result<number, DivisionErrors> {
  if (num === 0) {
    return Err(DivisionErrors.DivideByZero);
  }

  if (num === divisor) {
    return Err(DivisionErrors.NumberAndDivisorEqual);
  }

  return Ok(num / divisor);
}
```

Now our calling code can decide if it wants to handle this:

```typescript
function CallDivide(): number {
  let result = Divide(5, 0);

  if (result.ok) {
    return result.value;
  } else {
    return result.error === DivisionErrors.DivideByZero ? 0 : 1;
  }
}
```

Admittedly, we still rely on the calling code being _somewhat_ responsible, but at least we gave them the opportunity to be responsible! And if something _really_ goes wrong that our code definitely can't handle, we have no chance of getting caught up in it with a catch block.

Unfortunately for us though, we chose Typescript, which at the time of writing, doesn't have _pattern matching_, which would help us turn this up to eleven. Stay tuned for part 2, where we'll have a look at some other languages.
