import { Hono } from 'hono'
import { drizzle } from 'drizzle-orm/d1'
import { users, products, materialBatches, suppliers } from './db/schema'
import { eq } from 'drizzle-orm'
import { cors } from 'hono/cors'

type Bindings = {
  DB: D1Database
}

const app = new Hono<{ Bindings: Bindings }>()

app.use('*', cors())

app.get('/', (c) => {
  return c.text('GAIA API is running on Cloudflare Workers!')
})

app.get('/api/products', async (c) => {
  const db = drizzle(c.env.DB)
  const allProducts = await db.select().from(products).all()
  return c.json(allProducts)
})

app.get('/api/users', async (c) => {
  const db = drizzle(c.env.DB)
  const allUsers = await db.select().from(users).all()
  return c.json(allUsers)
})

export default app
