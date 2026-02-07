"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

interface Transaction {
  id: string;
  type: "withdraw" | "transfer" | "deposit";
  amount: number;
  description: string;
  date: string;
}

export default function BusinessDashboard() {
  const router = useRouter();
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [userName, setUserName] = useState("");
  const [userEmail, setUserEmail] = useState("");
  const [accountType, setAccountType] = useState("");
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [profit, setProfit] = useState("0.00");

  useEffect(() => {
    const loggedIn = localStorage.getItem("isLoggedIn");
    const storedAccountType = localStorage.getItem("accountType");

    if (!loggedIn) {
      router.push("/login");
      // } else if (storedAccountType !== 'business') {
      //   router.push('/')
    } else {
      setIsLoggedIn(true);
      setUserName(localStorage.getItem("userName") || "User");
      setUserEmail(localStorage.getItem("userEmail") || "");
      setAccountType(storedAccountType || "");

      const storedTransactions = localStorage.getItem("transactions");
      if (storedTransactions) {
        const allTransactions = JSON.parse(storedTransactions);
        setTransactions(allTransactions);

        // Calculate profit (deposits minus withdrawals and transfers)
        const totalDeposits = allTransactions
          .filter((t: Transaction) => t.type === "deposit")
          .reduce((sum: number, t: Transaction) => sum + t.amount, 0);

        const totalExpenses = allTransactions
          .filter(
            (t: Transaction) => t.type === "withdraw" || t.type === "transfer",
          )
          .reduce((sum: number, t: Transaction) => sum + t.amount, 0);

        const calculatedProfit = totalDeposits - totalExpenses;
        setProfit(calculatedProfit.toFixed(2));
      }
    }
  }, [router]);

  const handleLogout = () => {
    localStorage.removeItem("isLoggedIn");
    localStorage.removeItem("userEmail");
    localStorage.removeItem("userName");
    router.push("/login");
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

  const getTransactionTypeLabel = (type: string) => {
    switch (type) {
      case "deposit":
        return "Deposit";
      case "withdraw":
        return "Withdrawal";
      case "transfer":
        return "Transfer";
      default:
        return type;
    }
  };

  if (!isLoggedIn) {
    return null;
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <header className="border-b border-border bg-card">
        <div className="max-w-6xl mx-auto px-4 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
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
            </div>
            <h1 className="text-2xl font-bold">Bank System</h1>
          </div>
          <div className="flex items-center gap-3">
            <Link href="/">
              <Button
                variant="outline"
                className="border-border bg-transparent"
              >
                Back to Home
              </Button>
            </Link>
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  variant="outline"
                  className="border-border bg-transparent flex items-center gap-2"
                >
                  <div className="w-8 h-8 bg-primary rounded-full flex items-center justify-center">
                    <span className="text-sm font-bold text-primary-foreground">
                      {userName.charAt(0).toUpperCase()}
                    </span>
                  </div>
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                    strokeWidth={2}
                    stroke="currentColor"
                    className="w-4 h-4"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M19.5 8.25l-7.5 7.5-7.5-7.5"
                    />
                  </svg>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="center" className="w-md mt-2">
                <DropdownMenuLabel>My Accounts</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="flex-col items-start p-3 cursor-pointer">
                  <div className="flex items-center gap-3 w-full">
                    <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center flex-shrink-0">
                      <span className="text-sm font-bold text-primary-foreground">
                        {userName.charAt(0).toUpperCase()}
                      </span>
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="font-semibold text-sm truncate">
                        {userName}
                      </p>
                      <p className="text-xs text-muted-foreground truncate">
                        {userEmail}
                      </p>
                      <p className="text-xs text-muted-foreground mt-1">
                        🏢 Business Account
                      </p>
                    </div>
                  </div>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem
                  onClick={handleLogout}
                  className="cursor-pointer text-red-600"
                >
                  Logout
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-6xl mx-auto px-4 py-8">
        {/* Header Section */}
        <div className="mb-8">
          <h2 className="text-3xl font-bold mb-2">Business Dashboard</h2>
        </div>

        {/* Profit Card */}
        <Card className="mb-8 bg-primary border-primary">
          <CardHeader>
            <CardDescription className="text-primary-foreground/80">
              Total Profit
            </CardDescription>
            <CardTitle className="text-5xl font-bold text-primary-foreground">
              ${profit}
            </CardTitle>
          </CardHeader>
        </Card>

        {/* Transactions Table */}
        <Card>
          <CardHeader>
            <CardTitle className="text-2xl font-semibold">
              Transaction History
            </CardTitle>
          </CardHeader>
          <CardContent>
            {transactions.length === 0 ? (
              <p className="text-muted-foreground text-center py-8">
                No transactions yet
              </p>
            ) : (
              <div className="border border-border rounded-lg overflow-hidden">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Date & Time</TableHead>
                      <TableHead>Type</TableHead>
                      <TableHead>Description</TableHead>
                      <TableHead className="text-right">Amount</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {transactions.map((transaction) => (
                      <TableRow key={transaction.id}>
                        <TableCell className="font-medium">
                          {formatDate(transaction.date)}
                        </TableCell>
                        <TableCell>
                          <span
                            className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                              transaction.type === "deposit"
                                ? "bg-green-100 text-green-800"
                                : "bg-red-100 text-red-800"
                            }`}
                          >
                            {getTransactionTypeLabel(transaction.type)}
                          </span>
                        </TableCell>
                        <TableCell>{transaction.description}</TableCell>
                        <TableCell
                          className={`text-right font-bold ${
                            transaction.type === "deposit"
                              ? "text-green-600"
                              : "text-red-600"
                          }`}
                        >
                          {transaction.type === "deposit" ? "+" : "-"}$
                          {transaction.amount.toFixed(2)}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            )}
          </CardContent>
        </Card>
      </main>
    </div>
  );
}
