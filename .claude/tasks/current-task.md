# Current Task Management

## How to Use This File

This file tracks the active development focus. Update it when switching tasks.

**Priority levels:**
- `P0` — Blocking: system broken, must fix immediately
- `P1` — Critical: needed for current sprint/milestone
- `P2` — Important: scheduled for near-term
- `P3` — Nice-to-have: backlog candidate

---

## Active Task

**Task:** (Update this section when starting new work)

**Priority:** P1

**Service:** (Which microservice is being modified)

**Description:**
(What needs to be done and why)

**Affected Files:**
- (List files that will be created or modified)

**Acceptance Criteria:**
- [ ] (What defines "done")

**Dependencies:**
- (Other tasks or services this depends on)

---

## Recently Completed

| Date | Task | Service | Notes |
|---|---|---|---|
| 2026-04-21 | Product Image Management | Catalog | CRUD for product images with Cloudinary upload |
| 2026-04-20 | Catalog DTOs and API improvements | Catalog | Filter DTOs, pagination, new endpoints |
| 2026-04-19 | Catalog Migration | Catalog | Initial EF Core migration for catalog entities |
| 2026-04-18 | API services refactor | All | Unified ApiResponse pattern |

---

## Decision Log

Record important architectural decisions here:

| Date | Decision | Rationale |
|---|---|---|
| - | Service pattern over MediatR | Simpler for current scale, can migrate to CQRS+MediatR later |
| - | PostgreSQL for all services | Consistent tooling, JSONB for flexible fields if needed |
| - | Soft delete everywhere | Audit trail, data recovery, regulatory compliance |
| - | Cache-aside over write-through | Better for read-heavy workloads with tolerable staleness |
