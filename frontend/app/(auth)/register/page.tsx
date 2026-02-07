'use client'

import React from "react"

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import Link from 'next/link'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select'

export default function RegisterPage() {
  const router = useRouter()
  const [firstName, setFirstName] = useState('')
  const [lastName, setLastName] = useState('')
  const [nationalId, setNationalId] = useState('')
  const [residenceCardFront, setResidenceCardFront] = useState<File | null>(null)
  const [residenceCardBack, setResidenceCardBack] = useState<File | null>(null)
  const [nationalIdFront, setNationalIdFront] = useState<File | null>(null)
  const [nationalIdBack, setNationalIdBack] = useState<File | null>(null)
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [accountType, setAccountType] = useState('')

  const handleFileChange = (
    e: React.ChangeEvent<HTMLInputElement>,
    setter: React.Dispatch<React.SetStateAction<File | null>>
  ) => {
    if (e.target.files && e.target.files[0]) {
      setter(e.target.files[0])
    }
  }

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault()
    
    // Simple frontend validation
    if (
      firstName &&
      lastName &&
      nationalId &&
      residenceCardFront &&
      residenceCardBack &&
      nationalIdFront &&
      nationalIdBack &&
      email &&
      password &&
      phoneNumber &&
      accountType
    ) {
      // Initialize user account with demo balance
      localStorage.setItem('isLoggedIn', 'true')
      localStorage.setItem('userEmail', email)
      localStorage.setItem('userName', `${firstName} ${lastName}`)
      localStorage.setItem('balance', '5000.00')
      localStorage.setItem('accountType', accountType)
      localStorage.setItem('phoneNumber', phoneNumber)
      localStorage.setItem('nationalId', nationalId)
      
      // Initialize demo transactions
      const demoTransactions = [
        {
          id: '1',
          type: 'deposit',
          amount: 5000,
          description: 'Initial Deposit',
          date: new Date().toISOString()
        }
      ]
      localStorage.setItem('transactions', JSON.stringify(demoTransactions))
      
      router.push('/')
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4 py-12">
      <Card className="w-full max-w-2xl border-border shadow-lg">
        <CardHeader className="space-y-1 text-center">
          <div className="flex justify-center mb-4">
            <div className="w-16 h-16 bg-primary rounded-full flex items-center justify-center">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={2}
                stroke="currentColor"
                className="w-8 h-8 text-primary-foreground"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M12 21v-8.25M15.75 21v-8.25M8.25 21v-8.25M3 9l9-6 9 6m-1.5 12V10.332A48.36 48.36 0 0012 9.75c-2.551 0-5.056.2-7.5.582V21M3 21h18M12 6.75h.008v.008H12V6.75z"
                />
              </svg>
            </div>
          </div>
          <CardTitle className="text-3xl font-bold">SecureBank</CardTitle>
          <CardDescription className="text-base">Create your bank account</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleRegister} className="space-y-5">
            {/* Personal Information */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold border-b border-border pb-2">Personal Information</h3>
              
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="firstName">First Name</Label>
                  <Input
                    id="firstName"
                    type="text"
                    placeholder="First name"
                    value={firstName}
                    onChange={(e) => setFirstName(e.target.value)}
                    required
                    className="border-input"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="lastName">Last Name</Label>
                  <Input
                    id="lastName"
                    type="text"
                    placeholder="Last name"
                    value={lastName}
                    onChange={(e) => setLastName(e.target.value)}
                    required
                    className="border-input"
                  />
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="nationalId">National ID Number</Label>
                <Input
                  id="nationalId"
                  type="text"
                  placeholder="Enter your national ID number"
                  value={nationalId}
                  onChange={(e) => setNationalId(e.target.value)}
                  required
                  className="border-input"
                />
              </div>
            </div>

            {/* Document Uploads */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold border-b border-border pb-2">Document Uploads</h3>
              
              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="residenceCardFront">Residence Card (Front)</Label>
                  <Input
                    id="residenceCardFront"
                    type="file"
                    accept="image/*"
                    onChange={(e) => handleFileChange(e, setResidenceCardFront)}
                    required
                    className="border-input"
                  />
                  {residenceCardFront && (
                    <p className="text-xs text-muted-foreground">{residenceCardFront.name}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor="residenceCardBack">Residence Card (Back)</Label>
                  <Input
                    id="residenceCardBack"
                    type="file"
                    accept="image/*"
                    onChange={(e) => handleFileChange(e, setResidenceCardBack)}
                    required
                    className="border-input"
                  />
                  {residenceCardBack && (
                    <p className="text-xs text-muted-foreground">{residenceCardBack.name}</p>
                  )}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="nationalIdFront">National ID (Front)</Label>
                  <Input
                    id="nationalIdFront"
                    type="file"
                    accept="image/*"
                    onChange={(e) => handleFileChange(e, setNationalIdFront)}
                    required
                    className="border-input"
                  />
                  {nationalIdFront && (
                    <p className="text-xs text-muted-foreground">{nationalIdFront.name}</p>
                  )}
                </div>
                <div className="space-y-2">
                  <Label htmlFor="nationalIdBack">National ID (Back)</Label>
                  <Input
                    id="nationalIdBack"
                    type="file"
                    accept="image/*"
                    onChange={(e) => handleFileChange(e, setNationalIdBack)}
                    required
                    className="border-input"
                  />
                  {nationalIdBack && (
                    <p className="text-xs text-muted-foreground">{nationalIdBack.name}</p>
                  )}
                </div>
              </div>
            </div>

            {/* Contact & Account Information */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold border-b border-border pb-2">Contact & Account Information</h3>
              
              <div className="space-y-2">
                <Label htmlFor="email">Email Address</Label>
                <Input
                  id="email"
                  type="email"
                  placeholder="Enter your email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="border-input"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="phoneNumber">Phone Number</Label>
                <Input
                  id="phoneNumber"
                  type="tel"
                  placeholder="Enter your phone number"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                  required
                  className="border-input"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="password">Password</Label>
                <Input
                  id="password"
                  type="password"
                  placeholder="Create a password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="border-input"
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="accountType">Account Type</Label>
                <Select value={accountType} onValueChange={setAccountType} required>
                  <SelectTrigger className="border-input">
                    <SelectValue placeholder="Select account type" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="personal">Personal Account</SelectItem>
                    <SelectItem value="employee">Employee Account</SelectItem>
                    <SelectItem value="business">Business Account</SelectItem>
                    <SelectItem value="savings">Savings Account</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>

            <Button type="submit" className="w-full bg-primary text-primary-foreground hover:bg-primary/90 font-semibold">
              Register
            </Button>
          </form>
          <div className="mt-6 text-center text-sm">
            <span className="text-muted-foreground">Already have an account? </span>
            <Link href="/login" className="text-primary hover:underline font-medium">
              Login
            </Link>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
