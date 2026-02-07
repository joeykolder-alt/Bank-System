"use client";
import React from "react";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import { useRouter } from "next/navigation";

import Link from "next/link";

const Dropdown = () => {
  const router = useRouter();
  const handleLogout = () => {
    localStorage.removeItem("isLoggedIn");
    localStorage.removeItem("userEmail");
    localStorage.removeItem("userName");
    router.push("/login");
  };
  const userName = "Ali Jamal";
  const userEmail = "Alijmalnaser@gmail.com";
  type AccountType = "personal" | "business" | "employee" | "savings";

  const accountType: AccountType = "business";
  return (
    <>
      <div>
        <div className="flex items-center gap-3">
          {accountType === "business" && (
            <Link href="/business">
              <Button
                variant="outline"
                className="border-border bg-transparent"
              >
                Business
              </Button>
            </Link>
          )}
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button
                variant="outline"
                className="border-border bg-transparent flex items-center gap-2"
              >
                <div className="w-8 h-8 bg-primary rounded-full flex items-center justify-center">
                  <span className="text-sm font-bold text-primary-foreground">
                    {userName?.charAt(0).toUpperCase()}
                  </span>
                </div>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={2}
                  stroke="currentColor"
                  className="w-4 h-4 "
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
                      {userName?.charAt(0).toUpperCase()}
                    </span>
                  </div>
                  <div className="flex-1 min-w-0">
                    <p className="font-semibold text-sm truncate">{userName}</p>
                    <p className="text-xs text-muted-foreground truncate">
                      {userEmail}
                    </p>
                    <p className="text-xs text-muted-foreground mt-1 capitalize">
                      {accountType === "business" && "🏢 Business Account"}
                      {accountType === "personal" && "👤 Personal Account"}
                      {accountType === "employee" && "💼 Employee Account"}
                      {accountType === "savings" && "💰 Savings Account"}
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
    </>
  );
};

export default Dropdown;
