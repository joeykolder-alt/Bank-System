"use client";
import React from "react";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import Link from "next/link";

import Navbar from "@/component/main/Navbar";

interface Transaction {
  id: string;
  type: "withdraw" | "transfer" | "deposit";
  amount: number;
  description: string;
  date: string;
}

interface Account {
  id: string;
  name: string;
  type: "checking" | "savings";
  balance: number;
  number: string;
}

export default function HomePage() {
  const router = useRouter();
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [userName, setUserName] = useState("");
  const [balance, setBalance] = useState("1000000000.00");
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [withdrawAmount, setWithdrawAmount] = useState("");
  const [transferAmount, setTransferAmount] = useState("");
  const [transferRecipient, setTransferRecipient] = useState("");
  const [withdrawDialogOpen, setWithdrawDialogOpen] = useState(false);
  const [transferDialogOpen, setTransferDialogOpen] = useState(false);

  // Accounts State

  useEffect(() => {
    const loggedIn = localStorage.getItem("isLoggedIn");
    if (!loggedIn) {
      router.push("/login");
    } else {
      setIsLoggedIn(true);
      setUserName(localStorage.getItem("userName") || "User");
      setBalance(localStorage.getItem("balance") || "1000000000.00");

      const storedTransactions = localStorage.getItem("transactions");
      if (storedTransactions) {
        setTransactions(JSON.parse(storedTransactions));
      }
    }
  }, [router]);

  //السحب
  const handleWithdraw = (e: React.FormEvent) => {
    e.preventDefault();
    const amount = parseFloat(withdrawAmount); //قيمة السحب
    const currentBalance = parseFloat(balance); //الرصيد الحالي

    if (amount > 0 && amount <= currentBalance) {
      const newBalance = (currentBalance - amount).toFixed(2);
      const newTransaction: Transaction = {
        id: Date.now().toString(),
        type: "withdraw",
        amount: amount,
        description: "ATM Withdrawal",
        date: new Date().toISOString(),
      };

      const updatedTransactions = [newTransaction, ...transactions];

      localStorage.setItem("balance", newBalance);
      localStorage.setItem("transactions", JSON.stringify(updatedTransactions));

      setBalance(newBalance);
      setTransactions(updatedTransactions);
      setWithdrawAmount("");
      setWithdrawDialogOpen(false);
    }
  };
  //التحويل
  const handleTransfer = (e: React.FormEvent) => {
    e.preventDefault();
    const amount = parseFloat(transferAmount); //قيمة التحويل
    const currentBalance = parseFloat(balance); //الرصيد الحالي

    if (amount > 0 && amount <= currentBalance && transferRecipient) {
      const newBalance = (currentBalance - amount).toFixed(2);
      const newTransaction: Transaction = {
        id: Date.now().toString(),
        type: "transfer",
        amount: amount,
        description: `Transfer to ${transferRecipient}`,
        date: new Date().toISOString(),
      };

      const updatedTransactions = [newTransaction, ...transactions];

      localStorage.setItem("balance", newBalance);
      localStorage.setItem("transactions", JSON.stringify(updatedTransactions));

      setBalance(newBalance);
      setTransactions(updatedTransactions);
      setTransferAmount("");
      setTransferRecipient("");
      setTransferDialogOpen(false);
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      month: "short",
      day: "numeric",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  if (!isLoggedIn) {
    return null;
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <header className="border-b border-border bg-card">
        <div className="max-w-7xl mx-auto px-4 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
              <Link href="/">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={2}
                  stroke="currentColor"
                  className="w-6 h-6 text-primary-foreground"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M12 21v-8.25M15.75 21v-8.25M8.25 21v-8.25M3 9l9-6 9 6m-1.5 12V10.332A48.36 48.36 0 0012 9.75c-2.551 0-5.056.2-7.5.582V21M3 21h18M12 6.75h.008v.008H12V6.75z"
                  />
                </svg>
              </Link>
            </div>
            <h1 className="text-2xl font-bold">Bank System</h1>
          </div>
          <div className="flex gap-4">
          <Navbar />
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-6xl mx-auto px-4 py-8">
        {/* Welcome Section */}
        <div className="mb-8">
          <h2 className="text-3xl font-bold mb-2">Welcome back, {userName}!</h2>
        </div>

        {/* Balance Card */}
        <Card className="mb-8 bg-primary border-primary">
          <CardHeader>
            <CardDescription className="text-primary-foreground/80">
              Current Balance
            </CardDescription>
            <CardTitle className="text-5xl font-bold text-primary-foreground">
              ${balance}
            </CardTitle>
          </CardHeader>
          <CardContent className="flex gap-4">
            <Dialog
              open={withdrawDialogOpen}
              onOpenChange={setWithdrawDialogOpen}
            >
              <DialogTrigger asChild>
                <Button variant="secondary" className="flex-1 font-semibold">
                  Withdraw
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Withdraw Money</DialogTitle>
                  <DialogDescription>
                    Enter the amount you want to withdraw
                  </DialogDescription>
                </DialogHeader>
                <form onSubmit={handleWithdraw} className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="withdrawAmount">Amount ($)</Label>
                    <Input
                      id="withdrawAmount"
                      type="number"
                      step="0.01"
                      placeholder="0.00"
                      value={withdrawAmount}
                      onChange={(e) => setWithdrawAmount(e.target.value)}
                      required
                      min="0.01"
                      max={balance}
                    />
                  </div>
                  <Button
                    type="submit"
                    className="w-full bg-primary text-primary-foreground"
                  >
                    Confirm Withdrawal
                  </Button>
                </form>
              </DialogContent>
            </Dialog>

            <Dialog
              open={transferDialogOpen}
              onOpenChange={setTransferDialogOpen}
            >
              <DialogTrigger asChild>
                <Button variant="secondary" className="flex-1 font-semibold">
                  Transfer
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Transfer Money</DialogTitle>
                  <DialogDescription>Enter transfer details</DialogDescription>
                </DialogHeader>
                <form onSubmit={handleTransfer} className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="transferRecipient">Recipient</Label>
                    <Input
                      id="transferRecipient"
                      type="text"
                      placeholder="Recipient name or account"
                      value={transferRecipient}
                      onChange={(e) => setTransferRecipient(e.target.value)}
                      required
                    />
                  </div>
                  <div className="space-y-4">
                    <Label htmlFor="transferAmount">Amount ($)</Label>
                    <Input
                      id="transferAmount"
                      type="number"
                      step="0.01"
                      placeholder="0.00"
                      value={transferAmount}
                      onChange={(e) => setTransferAmount(e.target.value)}
                      required
                      min="0.01"
                      max={balance}
                    />
                    <Label>Note</Label>
                    <Input type="text" placeholder="note" required />
                  </div>
                  <Button
                    type="submit"
                    className="w-full bg-primary text-primary-foreground"
                  >
                    Confirm Transfer
                  </Button>
                </form>
              </DialogContent>
            </Dialog>
          </CardContent>
        </Card>

        {/* Recent Transactions */}
        <Card>
          <CardHeader>
            <CardTitle className="text-2xl font-semibold">Recent Transactions</CardTitle>
          </CardHeader>
          <CardContent>
            {transactions.length === 0 ? (
              <p className="text-muted-foreground text-center py-8">
                No transactions yet
              </p>
            ) : (
              <div className="space-y-4">
                {transactions.slice(0, 5).map((transaction) => (
                  <div
                    key={transaction.id}
                    className="flex items-center justify-between p-4 border border-border rounded-lg"
                  >
                    <div className="flex items-center gap-4">
                      <div
                        className={`w-10 h-10 rounded-full flex items-center justify-center ${
                          transaction.type === "deposit"
                            ? "bg-green-100"
                            : "bg-red-100"
                        }`}
                      >
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          strokeWidth={2}
                          stroke="currentColor"
                          className={`w-5 h-5 ${
                            transaction.type === "deposit"
                              ? "text-green-600"
                              : "text-red-600"
                          }`}
                        >
                          {transaction.type === "deposit" ? (
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              d="M12 4.5v15m0 0l6.75-6.75M12 19.5l-6.75-6.75"
                            />
                          ) : (
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              d="M12 19.5v-15m0 0l-6.75 6.75M12 4.5l6.75 6.75"
                            />
                          )}
                        </svg>
                      </div>
                      <div>
                        <p className="font-semibold">
                          {transaction.description}
                        </p>
                        <p className="text-sm text-muted-foreground">
                          {formatDate(transaction.date)}
                        </p>
                      </div>
                    </div>
                    <p
                      className={`font-bold text-lg ${
                        transaction.type === "deposit"
                          ? "text-green-600"
                          : "text-red-600"
                      }`}
                    >
                      {transaction.type === "deposit" ? "+" : "-"}$
                      {transaction.amount.toFixed(2)}
                    </p>
                  </div>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </main>
    </div>
  );
}
