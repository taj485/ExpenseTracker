# Expense Tracker — Test Plan

## Domain Tests

### Money

#### Creating Money
- [x] When I create money with a valid amount, it should return a Money object with that amount and GBP as the default currency
- [ ] When I create money with zero, it should throw an error
- [ ] When I create money with a negative amount, it should throw an error
- [ ] When I create money with an empty currency, it should throw an error

#### Adding Money
- [ ] When I add two amounts of the same currency together, it should return the correct total
- [ ] When I try to add two different currencies together, it should throw an error

---

### Expense

#### Creating an Expense
- [x] When I create an expense with a valid amount, category, and description, it should return a valid expense
- [x] When I create an expense with a negative amount, it should throw an error
- [x] When I create an expense with zero amount, it should throw an error
- [x] When I create an expense with an empty description, it should throw an error
- [ ] When I create an expense with a whitespace only description, it should throw an error
- [ ] When I create an expense, the date should be set automatically to today

#### Updating an Expense
- [x] When I update the amount with a valid value, it should update successfully 
- [x] When I update the amount with a negative value, it should throw an error
- [x] When I update the description with a valid value, it should update successfully
- [x] When I update the description with an empty value, it should throw an error

#### Deleting an Expense
- [ ]  When I delete an expense that has not been deleted, it should mark it as deleted
- [ ]  When I delete an expense that is already deleted, it should throw an error

---

## Application Tests

### AddExpense

#### Happy Path
- [x] When I send a valid add expense command, it should create and save the expense
- [x] When I send a valid add expense command, it should call the repository exactly once

#### Validation
- When I send a command with a negative amount, it should throw a validation error
- When I send a command with an empty description, it should throw a validation error
- When I send a command with an invalid category, it should throw a validation error

---

### GetExpenseById
- When I request an expense with a valid id, it should return the correct expense
- When I request an expense with an id that does not exist, it should throw a not found error

---

### GetAllExpenses
- When I request all expenses, it should return all non deleted expenses
- When there are no expenses, it should return an empty list

---

### DeleteExpense
- When I delete an expense with a valid id, it should mark it as deleted
- When I delete an expense that does not exist, it should throw a not found error
- When I delete an expense that is already deleted, it should throw an error

---

### GetMonthlySummary
- When I request a monthly summary, it should return the total of all expenses this month
- When there are no expenses this month, it should return zero
- When there are deleted expenses, they should not be included in the summary
