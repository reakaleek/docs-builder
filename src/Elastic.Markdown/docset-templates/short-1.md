# Couchbase Integration

# table with no borders

a | b
-- | -
0 | 1

# table 1 

Text before

| Field      | Description      | Type | Unit | Metric Type |
|------------|------------------|------|------|-------------|
| @timestamp | Event timestamp. | date | x    | x           |

# table 2

| Field | Description | Type | Unit | Metric Type |
|---|---|---|---|---|
| @timestamp | Event timestamp. | date |  |  |
| agent.id | Unique identifier of this agent (if one exists). Example: For Beats this would be beat.id. | keyword |  |  |
| cloud.account.id | The cloud account or organization id used to identify different entities in a multi-tenant environment. Examples: AWS account id, Google Cloud ORG Id, or other unique identifier. | keyword |  |  |

{variable_substitution}