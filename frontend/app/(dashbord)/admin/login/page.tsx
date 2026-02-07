'use client'

import React from "react"

import { useState } from 'react'
import { useRouter } from 'next/navigation'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

export default function AdminLoginPage() {
  const router = useRouter()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault()

    // Simple admin authentication (for demo purposes)
    // Default admin credentials: admin@securebank.com / admin123
    if (email === 'admin@securebank.com' && password === 'admin123') {
      localStorage.setItem('adminEmail', email)
      router.push('/admin')
    } else {
      alert('Invalid admin credentials')
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4">
      <Card className="w-full max-w-md border-border shadow-lg">
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
                  d="M16.5 10.5V6.75a4.5 4.5 0 10-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 002.25-2.25v-6.75a2.25 2.25 0 00-2.25-2.25H6.75a2.25 2.25 0 00-2.25 2.25v6.75a2.25 2.25 0 002.25 2.25z"
                />
              </svg>
            </div>
          </div>
          <CardTitle className="text-3xl font-bold">SecureBank</CardTitle>
          <CardDescription className="text-base">Admin Login</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleLogin} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="admin@securebank.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                className="border-input"
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Password</Label>
              <Input
                id="password"
                type="password"
                placeholder="Enter admin password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                className="border-input"
              />
            </div>
            <Button type="submit" className="w-full bg-primary text-primary-foreground hover:bg-primary/90 font-semibold">
              Login as Admin
            </Button>
          </form>
          {/* <div className="mt-6 p-4 bg-muted rounded-lg">
            <p className="text-xs text-muted-foreground text-center">
              <strong>Demo Credentials:</strong>
              <br />
              Email: admin@securebank.com
              <br />
              Password: admin123
            </p>
          </div> */}
        </CardContent>
      </Card>
    </div>
  )
}
