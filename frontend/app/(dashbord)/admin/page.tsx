'use client'

import { useEffect, useState } from 'react'
import { useRouter } from 'next/navigation'

import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import Link from 'next/link'

interface Transaction {
  id: string
  userId: string
  userName: string
  type: 'withdraw' | 'transfer' | 'deposit'
  amount: number
  description: string
  date: string
}

interface PendingRequest {
  id: string
  firstName: string
  lastName: string
  nationalId: string
  email: string
  password: string
  phoneNumber: string
  accountType: string
  status: string
  submittedAt: string
  residenceCardFront: string
  residenceCardBack: string
  nationalIdFront: string
  nationalIdBack: string
}

interface ApprovedUser {
  id: string
  firstName: string
  lastName: string
  email: string
  password: string
  phoneNumber: string
  nationalId: string
  accountType: string
  balance: string
  transactions: Transaction[]
}

export default function AdminDashboard() {
  const router = useRouter()
  const [isAdmin, setIsAdmin] = useState(false)
  const [bankBalance, setBankBalance] = useState('500000.00')
  const [allTransactions, setAllTransactions] = useState<Transaction[]>([])
  const [filteredTransactions, setFilteredTransactions] = useState<Transaction[]>([])
  const [searchQuery, setSearchQuery] = useState('')
  const [pendingRequests, setPendingRequests] = useState<PendingRequest[]>([])
  const [approvedUsers, setApprovedUsers] = useState<ApprovedUser[]>([])

  useEffect(() => {
    // Check if admin is logged in (for demo, we'll check if email is admin@securebank.com)
    const adminEmail = localStorage.getItem('adminEmail')
    if (adminEmail !== 'admin@securebank.com') {
      router.push('/admin/login')
    } else {
      setIsAdmin(true)
      loadData()
    }
  }, [router])

  const loadData = () => {
    // Load bank balance
    const storedBalance = localStorage.getItem('bankBalance')
    if (storedBalance) {
      setBankBalance(storedBalance)
    }

    // Load all user transactions
    const users = JSON.parse(localStorage.getItem('approvedUsers') || '[]')
    setApprovedUsers(users)

    const transactions: Transaction[] = []
    users.forEach((user: ApprovedUser) => {
      if (user.transactions) {
        user.transactions.forEach((transaction: Transaction) => {
          transactions.push({
            ...transaction,
            userId: user.id,
            userName: `${user.firstName} ${user.lastName}`,
          })
        })
      }
    })
    
    // Sort by date (newest first)
    transactions.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
    setAllTransactions(transactions)
    setFilteredTransactions(transactions)

    // Load pending requests
    const requests = JSON.parse(localStorage.getItem('pendingRequests') || '[]')
    setPendingRequests(requests.filter((req: PendingRequest) => req.status === 'pending'))
  }

  const handleSearch = (query: string) => {
    setSearchQuery(query)
    if (!query.trim()) {
      setFilteredTransactions(allTransactions)
    } else {
      const filtered = allTransactions.filter(
        (transaction) =>
          transaction.userName.toLowerCase().includes(query.toLowerCase()) ||
          transaction.description.toLowerCase().includes(query.toLowerCase()) ||
          transaction.type.toLowerCase().includes(query.toLowerCase())
      )
      setFilteredTransactions(filtered)
    }
  }

  const handleApprove = (request: PendingRequest) => {
    // Create new user account
    const newUser: ApprovedUser = {
      id: request.id,
      firstName: request.firstName,
      lastName: request.lastName,
      email: request.email,
      password: request.password,
      phoneNumber: request.phoneNumber,
      nationalId: request.nationalId,
      accountType: request.accountType,
      balance: '5000.00',
      transactions: [
        {
          id: Date.now().toString(),
          userId: request.id,
          userName: `${request.firstName} ${request.lastName}`,
          type: 'deposit',
          amount: 5000,
          description: 'Initial Deposit',
          date: new Date().toISOString(),
        },
      ],
    }

    // Add to approved users
    const updatedUsers = [...approvedUsers, newUser]
    setApprovedUsers(updatedUsers)
    localStorage.setItem('approvedUsers', JSON.stringify(updatedUsers))

    // Remove from pending requests
    const updatedRequests = pendingRequests.filter((req) => req.id !== request.id)
    setPendingRequests(updatedRequests)
    
    const allRequests = JSON.parse(localStorage.getItem('pendingRequests') || '[]')
    const updatedAllRequests = allRequests.map((req: PendingRequest) =>
      req.id === request.id ? { ...req, status: 'approved' } : req
    )
    localStorage.setItem('pendingRequests', JSON.stringify(updatedAllRequests))

    loadData()
  }

  const handleReject = (request: PendingRequest) => {
    // Remove from pending requests
    const updatedRequests = pendingRequests.filter((req) => req.id !== request.id)
    setPendingRequests(updatedRequests)
    
    const allRequests = JSON.parse(localStorage.getItem('pendingRequests') || '[]')
    const updatedAllRequests = allRequests.map((req: PendingRequest) =>
      req.id === request.id ? { ...req, status: 'rejected' } : req
    )
    localStorage.setItem('pendingRequests', JSON.stringify(updatedAllRequests))
  }

  const handleLogout = () => {
    localStorage.removeItem('adminEmail')
    router.push('/admin/login')
  }

  const formatDate = (dateString: string) => {
    const date = new Date(dateString)
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  if (!isAdmin) {
    return null
  }

  return (
    <div className="min-h-screen bg-background">
      {/* Header */}
      <header className="border-b border-border bg-card">
        <div className="max-w-7xl mx-auto px-4 py-4 flex items-center justify-between">
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
            <div>
              <Link href={"/"}>
              <h1 className="text-2xl font-bold">Bank System</h1>
              </Link>
              <p className="text-xs text-muted-foreground">Admin Dashboard</p>
            </div>
          </div>
          <Button variant="outline" onClick={handleLogout} className="border-border bg-transparent">
            Logout
          </Button>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 py-8">
        {/* Welcome Section */}
        <div className="mb-8">
          <h2 className="text-3xl font-bold mb-2">Welcome, Admin</h2>
        </div>

        {/* Bank Balance Card */}
        <Card className="mb-8 bg-primary border-primary">
          <CardHeader>
            <CardDescription className="text-primary-foreground/80">Bank Main Account Balance</CardDescription>
            <CardTitle className="text-5xl font-bold text-primary-foreground">${bankBalance}</CardTitle>
          </CardHeader>
        </Card>

        {/* Pending Registration Requests */}
        <Card className="mb-8">
          <CardHeader>
            <CardTitle className="flex items-center justify-between">
              Pending Registration Requests
              <Badge variant="secondary" className="text-base px-3 py-1">
                {pendingRequests.length}
              </Badge>
            </CardTitle>
           
          </CardHeader>
          <CardContent>
            {pendingRequests.length === 0 ? (
              <p className="text-muted-foreground text-center py-8">No pending requests</p>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="border-b border-border">
                    <tr className="text-left">
                      <th className="pb-3 pr-4 font-semibold">Name</th>
                      <th className="pb-3 pr-4 font-semibold">Email</th>
                      <th className="pb-3 pr-4 font-semibold">Phone</th>
                      <th className="pb-3 pr-4 font-semibold">Account Type</th>
                      <th className="pb-3 pr-4 font-semibold">National ID</th>
                      <th className="pb-3 pr-4 font-semibold">Submitted</th>
                      <th className="pb-3 font-semibold">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {pendingRequests.map((request) => (
                      <tr key={request.id} className="border-b border-border">
                        <td className="py-4 pr-4">
                          {request.firstName} {request.lastName}
                        </td>
                        <td className="py-4 pr-4 text-sm">{request.email}</td>
                        <td className="py-4 pr-4 text-sm">{request.phoneNumber}</td>
                        <td className="py-4 pr-4">
                          <Badge variant="outline" className="capitalize">
                            {request.accountType}
                          </Badge>
                        </td>
                        <td className="py-4 pr-4 text-sm">{request.nationalId}</td>
                        <td className="py-4 pr-4 text-sm text-muted-foreground">
                          {formatDate(request.submittedAt)}
                        </td>
                        <td className="py-4 flex gap-2">
                          <Button
                            size="sm"
                            onClick={() => handleApprove(request)}
                            className="bg-green-600 hover:bg-green-700 text-white"
                          >
                            Approve
                          </Button>
                          <Button
                            size="sm"
                            variant="destructive"
                            onClick={() => handleReject(request)}
                          >
                            Reject
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </CardContent>
        </Card>

        {/* All User Transactions */}
        <Card>
          <CardHeader>
            <CardTitle>All User Transactions</CardTitle>
            <div className="pt-4">
              <Input
                type="text"
                placeholder="Search by user name, description, or type..."
                value={searchQuery}
                onChange={(e) => handleSearch(e.target.value)}
                className="max-w-md"
              />
            </div>
          </CardHeader>
          <CardContent>
            {filteredTransactions.length === 0 ? (
              <p className="text-muted-foreground text-center py-8">
                {searchQuery ? 'No transactions found matching your search' : 'No transactions yet'}
              </p>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead className="border-b border-border">
                    <tr className="text-left">
                      <th className="pb-3 pr-4 font-semibold">User</th>
                      <th className="pb-3 pr-4 font-semibold">Type</th>
                      <th className="pb-3 pr-4 font-semibold">Description</th>
                      <th className="pb-3 pr-4 font-semibold">Date & Time</th>
                      <th className="pb-3 font-semibold text-right">Amount</th>
                    </tr>
                  </thead>
                  <tbody>
                    {filteredTransactions.map((transaction) => (
                      <tr key={transaction.id} className="border-b border-border">
                        <td className="py-4 pr-4 font-medium">{transaction.userName}</td>
                        <td className="py-4 pr-4">
                          <Badge
                            variant="outline"
                            className={`capitalize ${
                              transaction.type === 'deposit'
                                ? 'border-green-500 text-green-700'
                                : 'border-red-500 text-red-700'
                            }`}
                          >
                            {transaction.type}
                          </Badge>
                        </td>
                        <td className="py-4 pr-4 text-sm">{transaction.description}</td>
                        <td className="py-4 pr-4 text-sm text-muted-foreground">
                          {formatDate(transaction.date)}
                        </td>
                        <td
                          className={`py-4 font-bold text-right ${
                            transaction.type === 'deposit' ? 'text-green-600' : 'text-red-600'
                          }`}
                        >
                          {transaction.type === 'deposit' ? '+' : '-'}${transaction.amount.toFixed(2)}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </CardContent>
        </Card>
      </main>
    </div>
  )
}
