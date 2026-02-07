"use client";
import React, { useState } from "react";
import { Button } from "@/components/ui/button";

import { Bell, Briefcase, DollarSign, PiggyBank, User } from "lucide-react";
import { useRouter } from "next/navigation";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import Dropdown from "./Dropdown";

function Navbar() {
  const [newAccountName, setNewAccountName] = useState("");
  const [newAccountPhone, setNewAccountPhone] = useState("");
  const [newAccountType, setNewAccountType] = useState("");
  const [addAccountDialogOpen, setAddAccountDialogOpen] = useState(false);
  const handleCreateAccount = (e: React.FormEvent) => {
    e.preventDefault();
    if (newAccountName && newAccountPhone && newAccountType) {
      alert(
        `Account created successfully!\nName: ${newAccountName}\nType: ${newAccountType}`,
      );
      setNewAccountName("");
      setNewAccountPhone("");
      setNewAccountType("");
      setAddAccountDialogOpen(false);
    }
  };
  return (
    <>
      <Dialog
        open={addAccountDialogOpen}
        onOpenChange={setAddAccountDialogOpen}
      >
        <DialogTrigger asChild>
          <Button size="icon" className="border-border rounded-full">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={2}
              stroke="currentColor"
              className="w-5 h-5"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M12 4.5v15m7.5-7.5h-15"
              />
            </svg>
          </Button>
        </DialogTrigger>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader className="space-y-3 pb-4">
            <DialogTitle className="text-2xl font-bold">
              Add New Account
            </DialogTitle>
            <DialogDescription className="text-base">
              Add a new account to your profile.
            </DialogDescription>
          </DialogHeader>
          <form onSubmit={handleCreateAccount} className="space-y-6">
            <div className="space-y-2">
              <Label htmlFor="accountName" className="text-sm font-medium">
                Name
              </Label>
              <Input
                id="accountName"
                type="text"
                placeholder="Enter account name"
                value={newAccountName}
                onChange={(e) => setNewAccountName(e.target.value)}
                required
                className="h-11"
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="accountPhone" className="text-sm font-medium">
                Phone Number
              </Label>
              <Input
                id="accountPhone"
                type="tel"
                placeholder="Enter phone number"
                value={newAccountPhone}
                onChange={(e) => setNewAccountPhone(e.target.value)}
                required
                className="h-11"
              />
            </div>

            <div className="space-y-3">
              <Label
                htmlFor="accountTypeSelect"
                className="text-sm font-medium"
              >
                Account Type
              </Label>
              <Select
                value={newAccountType}
                onValueChange={setNewAccountType}
                required
              >
                <SelectTrigger className="h-11">
                  <SelectValue placeholder="Select account type" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="personal">
                    <div className="flex items-center gap-3 py-2">
                      <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          strokeWidth={2}
                          stroke="currentColor"
                          className="w-5 h-5 text-primary"
                        >
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z"
                          />
                        </svg>
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="font-semibold text-sm">
                          Personal Account
                        </p>
                        <p className="text-xs text-muted-foreground">
                          Manage your daily transactions
                        </p>
                      </div>
                    </div>
                  </SelectItem>
                  <SelectItem value="savings">
                    <div className="flex items-center gap-3 py-2">
                      <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          strokeWidth={2}
                          stroke="currentColor"
                          className="w-5 h-5 text-primary"
                        >
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            d="M11.48 3.499a.562.562 0 011.04 0l2.125 5.111a.563.563 0 00.475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 00-.182.557l1.285 5.385a.562.562 0 01-.84.61l-4.725-2.885a.563.563 0 00-.586 0L6.982 20.54a.562.562 0 01-.84-.61l1.285-5.386a.562.562 0 00-.182-.557l-4.204-3.602a.563.563 0 01.321-.988l5.518-.442a.563.563 0 00.475-.345L11.48 3.5z"
                          />
                        </svg>
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="font-semibold text-sm">Saving Account</p>
                        <p className="text-xs text-muted-foreground">
                          Earn interest on your savings
                        </p>
                      </div>
                    </div>
                  </SelectItem>
                  <SelectItem value="business">
                    <div className="flex items-center gap-3 py-2">
                      <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          strokeWidth={2}
                          stroke="currentColor"
                          className="w-5 h-5 text-primary"
                        >
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            d="M12 6v12m-3-2.818l.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.45-.22-2.003-.659-1.106-.879-1.106-2.303 0-3.182s2.9-.879 4.006 0l.415.33M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="font-semibold text-sm">
                          Business Account
                        </p>
                        <p className="text-xs text-muted-foreground">
                          Manage your business transactions
                        </p>
                      </div>
                    </div>
                  </SelectItem>
                  <SelectItem value="employee">
                    <div className="flex items-center gap-3 py-2">
                      <div className="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          fill="none"
                          viewBox="0 0 24 24"
                          strokeWidth={2}
                          stroke="currentColor"
                          className="w-5 h-5 text-primary"
                        >
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            d="M20.25 14.15v4.25c0 1.094-.787 2.036-1.872 2.18-2.087.277-4.216.42-6.378.42s-4.291-.143-6.378-.42c-1.085-.144-1.872-1.086-1.872-2.18v-4.25m16.5 0a2.18 2.18 0 00.75-1.661V8.706c0-1.081-.768-2.015-1.837-2.175a48.114 48.114 0 00-3.413-.387m4.5 8.006c-.194.165-.42.295-.673.38A23.978 23.978 0 0112 15.75c-2.648 0-5.195-.429-7.577-1.22a2.016 2.016 0 01-.673-.38m0 0A2.18 2.18 0 013 12.489V8.706c0-1.081.768-2.015 1.837-2.175a48.111 48.111 0 013.413-.387m7.5 0V5.25A2.25 2.25 0 0013.5 3h-3a2.25 2.25 0 00-2.25 2.25v.894m7.5 0a48.667 48.667 0 00-7.5 0M12 12.75h.008v.008H12v-.008z"
                          />
                        </svg>
                      </div>
                      <div className="flex-1 min-w-0">
                        <p className="font-semibold text-sm">
                          Employee Account
                        </p>
                        <p className="text-xs text-muted-foreground">
                          Manage your employee transactions
                        </p>
                      </div>
                    </div>
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="flex gap-3 pt-4">
              <Button
                type="button"
                variant="outline"
                onClick={() => setAddAccountDialogOpen(false)}
                className="flex-1 h-11"
              >
                Cancel
              </Button>
              <Button
                type="submit"
                className="flex-1 h-11 bg-primary text-primary-foreground hover:bg-primary/90 font-semibold"
              >
                Create Account
              </Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
      <div className="flex gap-4">
        <Button variant="outline">
          <Bell />
        </Button>
        <Dropdown />
      </div>
    </>
  );
}

export default Navbar;
