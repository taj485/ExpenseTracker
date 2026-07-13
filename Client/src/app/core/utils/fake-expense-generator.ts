import { ExpenseCategory, ExtractedExpense } from '../models/expense.model';
import { todayLocalISODate } from './date.utils';

interface FakeExpenseTemplate {
  description: string;
  category: ExpenseCategory;
  minAmount: number;
  maxAmount: number;
  maxQuantity: number;
}

const TEMPLATES: FakeExpenseTemplate[] = [
  { description: 'Coffee',               category: 'Food',          minAmount: 2.5,  maxAmount: 5.5,  maxQuantity: 2 },
  { description: 'Sandwich',             category: 'Food',          minAmount: 4,    maxAmount: 8.5,  maxQuantity: 2 },
  { description: 'Groceries',            category: 'Food',          minAmount: 15,   maxAmount: 45,   maxQuantity: 1 },
  { description: 'Bus fare',             category: 'Transport',     minAmount: 2,    maxAmount: 4.5,  maxQuantity: 2 },
  { description: 'Taxi ride',            category: 'Transport',     minAmount: 8,    maxAmount: 22,   maxQuantity: 1 },
  { description: 'Parking',              category: 'Transport',     minAmount: 3,    maxAmount: 10,   maxQuantity: 1 },
  { description: 'Electricity top-up',   category: 'Utilities',     minAmount: 10,   maxAmount: 30,   maxQuantity: 1 },
  { description: 'Mobile top-up',        category: 'Utilities',     minAmount: 5,    maxAmount: 20,   maxQuantity: 1 },
  { description: 'Cinema ticket',        category: 'Entertainment', minAmount: 8,    maxAmount: 15,   maxQuantity: 3 },
  { description: 'Streaming subscription', category: 'Entertainment', minAmount: 5.99, maxAmount: 14.99, maxQuantity: 1 },
  { description: 'Pharmacy items',       category: 'Health',        minAmount: 4,    maxAmount: 25,   maxQuantity: 2 },
  { description: 'Vitamins',             category: 'Health',        minAmount: 6,    maxAmount: 18,   maxQuantity: 1 },
];

function randomInt(min: number, max: number): number {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

function randomAmount(min: number, max: number): number {
  return Math.round((Math.random() * (max - min) + min) * 100) / 100;
}

export function generateFakeExpenses(): ExtractedExpense[] {
  const count = randomInt(2, 4);
  const shuffled = [...TEMPLATES].sort(() => Math.random() - 0.5);
  const today = todayLocalISODate();

  return shuffled.slice(0, count).map(t => ({
    amount: randomAmount(t.minAmount, t.maxAmount),
    category: t.category,
    description: t.description,
    date: today,
    quantity: randomInt(1, t.maxQuantity),
  }));
}
