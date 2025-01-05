<script lang="ts">
  const operations = [
    {
      label: '+',
      op: (a: number, b: number) => a + b
    },
    {
      label: '-',
      op: (a: number, b: number) => a - b
    },
    {
      label: '*',
      op: (a: number, b: number) => a * b
    },
    {
      label: '/',
      op: (a: number, b: number) => {
        if (a % b != 0) {
          return NaN;
        }
        return Math.trunc(a / b);
      }
    }
  ];
  let target = $state(15);
  let firstLoad = $state(true);
  let carriageNumber: number = $state();
  let terms: number[] = $derived(getTerms(carriageNumber));

  let options: string[][] = $derived(getOptions(target, terms));

  $effect(() => {
    while (firstLoad && !options.length) {
      let n = getRandomCarriageNumber();
      let t = getTerms(n);
      if (getOptions(target, t).length) {
        carriageNumber = n;
        firstLoad = false;
        break;
      }
    }
  });

  function getRandomCarriageNumber() {
    return Math.floor(Math.random() * (999999 - 100000 + 1) + 100000);
  }

  function getOptions(target: number, terms: number[]): string[][] {
    const ret = [];
    const stack: [{ terms: number[]; ops: (string | number)[] }] = [{ terms, ops: [terms[0]] }];
    while (stack.length) {
      const step = stack.pop();
      if (step.terms.length === 1) {
        if (step.terms[0] === target) {
          ret.push(step.ops);
        }
        continue;
      }

      const a = step.terms[0];
      const b = step.terms[1];
      const rest = step.terms.slice(2);
      for (const op of operations) {
        const result = op.op(a, b);
        if (!isNaN(result)) {
          stack.push({ terms: [result, ...rest], ops: [...step.ops, op.label, b] });
        }
      }
    }

    return ret;
  }

  function getTerms(n: number) {
    return (n || '')
      .toString()
      .split('')
      .map((c) => +c);
  }
</script>

<section class="container">
  <h2>Carriage Calculator</h2>
  <sl-input
    class="form-input"
    type="number"
    value={target}
    oninput={(e) => (target = Number(e.target.value))}
    label="Target"
    placeholder="Enter a number"
    inputmode="numeric"
    size="large"><sl-icon name="bullseye" slot="prefix"></sl-icon></sl-input
  >

  <sl-input
    class="form-input"
    type="number"
    value={carriageNumber}
    oninput={(e) => (carriageNumber = Number(e.target.value))}
    label="Carriage Number"
    placeholder="Enter a number"
    inputmode="numeric"
    size="large"><sl-icon name="train-front" slot="prefix"></sl-icon></sl-input
  >

  <section class="options">
    {#if options.length}
      <h3>Options</h3>
      <ul class="option-list">
        {#each options as option}
          <li class="option">
            <span class="target">{target}</span>
            <span class="equals">=</span>
            <span class="terms">
              {#each option as term, index}
                <span class="term{index % 2 != 0 ? ' op' : ''}">{term}</span>
              {/each}
            </span>
          </li>
        {/each}
      </ul>
    {/if}
  </section>
  <section class="about">
    <h4>About</h4>
    <p>
      On a trip to London a friend taught me a mental game she plays to keep herself busy on the
      tube.
    </p>
    <p>
      The game is simple: look at the carriage number, and use the digits in it to get to the number
      15.
    </p>
    <p>
      This happened to be the exact problem to solve in <a
        href="https://adventofcode.com/2024/day/7"
        rel="noopener nofollow">Day 7 2024 of Advent of Code</a
      >, so I thought, I'd throw together this little website to play it for me.
    </p>
    <p>The digits are evaluated left to right in order, without operator precedence.</p>
  </section>
</section>

<style>
  .container {
    display: flex;
    flex-direction: column;
    margin-left: auto;
    margin-right: auto;
    max-width: 800px;
    gap: 1rem;
  }

  .form-input::part(form-control-input) {
    --sl-input-font-family: monospace;
  }

  .option-list {
    list-style: none;
    line-height: 2;
  }

  .option {
    font-size: larger;
    display: flex;
    flex-direction: row;
    flex-wrap: nowrap;
    gap: 1rem;
  }

  .terms {
    display: flex;
    flex-direction: row;
    gap: 0.5rem;
  }

  .term.op {
    font-weight: bold;
  }
</style>
