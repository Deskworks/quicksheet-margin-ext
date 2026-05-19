# quicksheet-margin-ext

A [QuickSheet](https://github.com/cemheren/QuickSheet) extension that calculates **break-even point** and **contribution margin** — essential metrics for pricing decisions and business planning.

## What it does

Enter your selling price, variable cost per unit, and total fixed costs. The extension instantly shows:

- **Contribution margin** per unit and as a percentage
- **Break-even point** in units and revenue
- **Health indicator** (🟢 strong → 🔴 razor-thin margin)

## Usage

In any QuickSheet cell, type:

```
margin: 50, 20, 9000
```

Parameters: `sellingPrice, variableCostPerUnit, totalFixedCosts`

### Output (3×2 grid)

```
🟢 Strong margin          CM: $30.00/unit (60.0%)
Break-even: 300 units     BE revenue: $15,000.00
Fixed: $9,000.00          Price $50.00 − Var $20.00
```

### Example: SaaS pricing analysis

```csv
Plan,Price,Infra Cost,Fixed,Analysis
Starter,29,8,50000,"margin: 29, 8, 50000"
Pro,79,15,50000,"margin: 79, 15, 50000"
Enterprise,199,25,50000,"margin: 199, 25, 50000"
```

## Margin health thresholds

| Ratio | Status | Meaning |
|-------|--------|---------|
| ≥ 60% | 🟢 Strong | Healthy pricing power |
| ≥ 40% | 🟡 Moderate | Typical for competitive markets |
| ≥ 20% | 🟠 Thin | Watch costs carefully |
| < 20% | 🔴 Razor-thin | Consider repricing |

## Install

Clone into your QuickSheet extensions directory:

```bash
cd ~/.quicksheet/extensions
git clone https://github.com/Deskworks/quicksheet-margin-ext.git
```

Requires .NET 9 SDK. Zero NuGet dependencies.

## Part of the QuickSheet Accounting Suite

Works alongside other financial extensions:
- [quicksheet-budget](https://github.com/Deskworks/quicksheet-budget) — budget envelope visualizer
- [quicksheet-qtr](https://github.com/Deskworks/quicksheet-qtr) — quarterly tax deadline countdown
- [quicksheet-fx](https://github.com/Deskworks/quicksheet-fx) — live currency conversion
- [quicksheet-1099-ext](https://github.com/Deskworks/quicksheet-1099-ext) — freelancer tax estimator
- [quicksheet-mileage-ext](https://github.com/Deskworks/quicksheet-mileage-ext) — IRS standard mileage

## License

MIT
